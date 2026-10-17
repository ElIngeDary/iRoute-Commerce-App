# iRoute Solutions - Gestión y Cuarentena de Comercios

Aplicación Full Stack para la carga masiva de comercios mediante archivos CSV, procesamiento y filtrado por fecha con desvío a cuarentena de registros inválidos o duplicados.

## 🚀 Tecnologías Utilizadas

- **Frontend** Angular 18 (Standalone Components), Bootstrap 5, RxJS
- **Backend:** .NET 8 Web API, Entity Framework Core / SQL Client
- **Base de Datos:** SQL Server 2022 (Tablas y Stored Procedures)
- **Autenticación:** Autenticación básica con `LocalStorage` y `AuthGuard`

## 🛠️ Arquitectura y Funcionalidades

1. **Carga CSV:** Subida masiva de archivos con formato `multipart/form-data`.
2. **Procesamiento Batch:** Procedimiento almacenado en SQL Server que valida reglas de negocio (campos obligatorios vacíos, caracteres especiales en documentos, etc.).
3. **Módulo de Cuarentena:** Interfaz interactiva que reporta los registros rechazados y especifica el motivo del fallo.
4. **Seguridad y Rutas:** Pantalla de Login y protección de rutas privadas (`/upload`, `/process`, `/quarantine`).

## ⚙️ Instrucciones de Ejecución

### 1. Base de Datos
- Ejecutar el script SQL para la creación de las tablas y procedimientos almacenados.
- Configurar la cadena de conexión en `CommerceApi/appsettings.json`.

### 2. Backend (.NET 8)
```bash
cd CommerceApi
dotnet restore
dotnet run

### ---------------------------------------
API escuchando en: http://localhost:5107
### ---------------------------------------

## 3. Para Activar Frontend (Angular 18)

cd CommerceUI
npm install
ng serve -o


### ---------------------------------------
Aplicación en: http://localhost:4200
### ---------------------------------------

### ---------------------------------------
Usuario de prueba: admin | Contraseña: 123456
### ---------------------------------------

