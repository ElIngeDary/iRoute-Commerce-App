using System.Data;
using CommerceApi.Dtos;
using Microsoft.Data.SqlClient;

namespace CommerceApi.Repositories
{
    public class CommerceRepository : ICommerceRepository
    {
        private readonly string _connectionString;

        public CommerceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        public async Task BulkInsertCommerceAsync(IEnumerable<CommerceCsvDto> records)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var record in records)
                {
                    using var command = new SqlCommand("dbo.sp_create_commerce", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@pc_nomcomred", (object?)record.PcNomcomred ?? DBNull.Value);
                    command.Parameters.AddWithValue("@pc_numdoc", (object?)record.PcNumdoc ?? DBNull.Value);
                    command.Parameters.AddWithValue("@pc_processdate", record.PcProcessdate);

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> ProcessCommerceByDateAsync(DateTime processDate)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("dbo.sp_process_commerce", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@pc_processdate", processDate);

            var result = await command.ExecuteScalarAsync();
            return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
        }

        public async Task<IEnumerable<CommerceQuarantineDto>> GetQuarantineListAsync()
        {
            var list = new List<CommerceQuarantineDto>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT id, pc_nomcomred, pc_numdoc, pc_processdate, motivo, fecha_cuarentena 
                FROM dbo.commerce_quarantine 
                ORDER BY fecha_cuarentena DESC";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new CommerceQuarantineDto
                {
                    Id = reader.GetInt32(0),
                    PcNomcomred = reader.IsDBNull(1) ? null : reader.GetString(1),
                    PcNumdoc = reader.IsDBNull(2) ? null : reader.GetString(2),
                    PcProcessdate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    Motivo = reader.GetString(4),
                    FechaCuarentena = reader.GetDateTime(5)
                });
            }

            return list;
        }
    }
}