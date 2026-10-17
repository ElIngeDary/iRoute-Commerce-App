using CommerceApi.Dtos;

namespace CommerceApi.Services
{
    public interface ICommerceService
    {
        Task<int> ProcessCsvFileAsync(IFormFile file);
        Task<int> ProcessCommerceByDateAsync(DateTime processDate);
        Task<IEnumerable<CommerceQuarantineDto>> GetQuarantineListAsync();
    }
}