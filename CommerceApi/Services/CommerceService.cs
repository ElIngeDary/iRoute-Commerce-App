using System.Globalization;
using CommerceApi.Dtos;
using CommerceApi.Repositories;

namespace CommerceApi.Services
{
    public class CommerceService : ICommerceService
    {
        private readonly ICommerceRepository _repository;

        public CommerceService(ICommerceRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> ProcessCsvFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("El archivo CSV enviado está vacío.");
            }

            var records = new List<CommerceCsvDto>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string? line;
                bool isHeader = true;
                int lineNumber = 0;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    var columns = line.Split(',');

                    if (columns.Length < 3)
                    {
                        throw new FormatException($"La línea {lineNumber} no tiene al menos 3 columnas separadas por coma.");
                    }

                    string nomcomred = columns[0].Trim();
                    string numdoc = columns[1].Trim();
                    string dateStr = columns[2].Trim();

                    
                    string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy/MM/dd", "MM/dd/yyyy", "dd-MM-yyyy" };

                    if (!DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        
                        if (!DateTime.TryParse(dateStr, out parsedDate))
                        {
                            throw new FormatException($"La fecha '{dateStr}' en la línea {lineNumber} tiene un formato no reconocido. Use yyyy-MM-dd.");
                        }
                    }

                    records.Add(new CommerceCsvDto
                    {
                        PcNomcomred = string.IsNullOrEmpty(nomcomred) ? null : nomcomred,
                        PcNumdoc = string.IsNullOrEmpty(numdoc) ? null : numdoc,
                        PcProcessdate = parsedDate
                    });
                }
            }

            if (records.Count == 0)
            {
                throw new ArgumentException("El archivo CSV no contiene registros de datos válidos.");
            }

            
            await _repository.BulkInsertCommerceAsync(records);

            return records.Count;
        }

        public async Task<int> ProcessCommerceByDateAsync(DateTime processDate)
        {
            return await _repository.ProcessCommerceByDateAsync(processDate);
        }

        public async Task<IEnumerable<CommerceQuarantineDto>> GetQuarantineListAsync()
        {
            return await _repository.GetQuarantineListAsync();
        }
    }
}