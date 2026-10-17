-- ============================================================================
-- SCRIPT DE BASE DE DATOS - PRUEBA TÉCNICA iRoute Solutions
-- Motor: SQL Server 2021
-- ============================================================================


--- Creación y Configuración inicial de la Base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'iRouteDB')
BEGIN
    CREATE DATABASE iRouteDB;
END
GO

USE iRouteDB;
GO

-- ----------------------------------------------------------------------------
-- 1. TABLAS PRINCIPALES
-- ----------------------------------------------------------------------------

-- Tabla principal de almacenamiento de comercios
IF OBJECT_ID('dbo.commerce', 'U') IS NOT NULL DROP TABLE dbo.commerce;
GO

CREATE TABLE dbo.commerce (
    id INT IDENTITY(1,1) NOT NULL,
    pc_nomcomred VARCHAR(255) NULL,
    pc_numdoc VARCHAR(50) NULL,
    pc_processdate DATE NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT PK_commerce PRIMARY KEY CLUSTERED (id)
);
GO

-- Índice no agrupado para acelerar las consultas de procesamiento filtradas por fecha
CREATE NONCLUSTERED INDEX IX_commerce_processdate 
ON dbo.commerce (pc_processdate) 
INCLUDE (pc_nomcomred, pc_numdoc);
GO


-- Tabla de cuarentena para registros con errores de validación
IF OBJECT_ID('dbo.commerce_quarantine', 'U') IS NOT NULL DROP TABLE dbo.commerce_quarantine;
GO

CREATE TABLE dbo.commerce_quarantine (
    id INT IDENTITY(1,1) NOT NULL,
    pc_nomcomred VARCHAR(255) NULL,
    pc_numdoc VARCHAR(50) NULL,
    pc_processdate DATE NULL,
    motivo VARCHAR(500) NOT NULL,
    fecha_cuarentena DATETIME DEFAULT GETDATE(),

    CONSTRAINT PK_commerce_quarantine PRIMARY KEY CLUSTERED (id)
);
GO

-- ----------------------------------------------------------------------------
-- 2. PROCEDIMIENTOS ALMACENADOS (STORED PROCEDURE - SP)
-- ----------------------------------------------------------------------------

-- SP 1: Registro de comercio individual desde la API
IF OBJECT_ID('dbo.sp_create_commerce', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_create_commerce;
GO

CREATE PROCEDURE dbo.sp_create_commerce
    @pc_nomcomred VARCHAR(255) = NULL,
    @pc_numdoc VARCHAR(50) = NULL,
    @pc_processdate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        INSERT INTO dbo.commerce (pc_nomcomred, pc_numdoc, pc_processdate)
        VALUES (@pc_nomcomred, @pc_numdoc, @pc_processdate);
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- SP 2: Procesamiento por fecha, detección de errores, migración a cuarentena y borrado
IF OBJECT_ID('dbo.sp_process_commerce', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_process_commerce;
GO

CREATE PROCEDURE dbo.sp_process_commerce
    @pc_processdate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Prevenir bloqueos innecesarios usando transacción explícita
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Tabla temporal para almacenar los registros observados y evaluar motivos
        CREATE TABLE #InvalidRecords (
            id INT NOT NULL,
            pc_nomcomred VARCHAR(255) NULL,
            pc_numdoc VARCHAR(50) NULL,
            pc_processdate DATE NULL,
            motivo VARCHAR(500) NOT NULL
        );

        -- Identificación y categorización de motivos usando CASE
        INSERT INTO #InvalidRecords (id, pc_nomcomred, pc_numdoc, pc_processdate, motivo)
        SELECT 
            id,
            pc_nomcomred,
            pc_numdoc,
            pc_processdate,
            CASE 
                -- Caso 1: Ambos campos con error
                WHEN (pc_nomcomred IS NULL OR LTRIM(RTRIM(pc_nomcomred)) = '') 
                     AND (pc_numdoc IS NULL OR LTRIM(RTRIM(pc_numdoc)) = '' OR pc_numdoc LIKE '%[^0-9]%') 
                THEN 'El nombre del comercio (nomcomred) se encuentra vacío y el número (numdoc) es inválido o contiene caracteres no numéricos.'

                -- Caso 2: Nombre de comercio vacío
                WHEN (pc_nomcomred IS NULL OR LTRIM(RTRIM(pc_nomcomred)) = '') 
                THEN 'El nombre del comercio (nomcomred) se encuentra vacío'

                -- Caso 3: Documento vacío
                WHEN (pc_numdoc IS NULL OR LTRIM(RTRIM(pc_numdoc)) = '') 
                THEN 'El número de documento (numdoc) se encuentra vacío'

                -- Caso 4: Documento con letras o caracteres especiales
                WHEN pc_numdoc LIKE '%[^0-9]%' 
                THEN 'El número (numdoc) contiene letras o caracteres especiales'
            END AS motivo
        FROM dbo.commerce
        WHERE pc_processdate = @pc_processdate
          AND (
                pc_nomcomred IS NULL 
                OR LTRIM(RTRIM(pc_nomcomred)) = '' 
                OR pc_numdoc IS NULL 
                OR LTRIM(RTRIM(pc_numdoc)) = ''
                OR pc_numdoc LIKE '%[^0-9]%'
              );

        -- Insertar en la tabla de cuarentena
        INSERT INTO dbo.commerce_quarantine (pc_nomcomred, pc_numdoc, pc_processdate, motivo)
        SELECT pc_nomcomred, pc_numdoc, pc_processdate, motivo
        FROM #InvalidRecords;

        DECLARE @InsertedQuarantineCount INT = @@ROWCOUNT;

        -- Eliminar de la tabla de commerce los registros procesados con error
        DELETE FROM dbo.commerce
        WHERE id IN (SELECT id FROM #InvalidRecords);

        -- Limpiar tabla temporal
        DROP TABLE #InvalidRecords;

        COMMIT TRANSACTION;

        -- Retornar el total de registros movidos a cuarentena
        SELECT @InsertedQuarantineCount AS QuarantineCount;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        THROW;
    END CATCH
END;
GO