using CommerceApi.Dtos;

namespace CommerceApi.Repositories
{
    public interface ICommerceRepository
    {
        Task BulkInsertCommerceAsync(IEnumerable<CommerceCsvDto> records);
        Task<int> ProcessCommerceByDateAsync(DateTime processDate);
        Task<IEnumerable<CommerceQuarantineDto>> GetQuarantineListAsync();
    }
}