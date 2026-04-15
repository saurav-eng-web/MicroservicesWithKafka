using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IFundRepository
    {
        Task<List<Fund>> GetAllFunds();
        Task<(List<Fund> Items, int TotalCount)> GetPagedFunds(int page, int pageSize);
        Task<Fund> GetFundByID(int id);
        Task<List<Fund>> FilterFundsByField(string fieldName, string fieldValue);

        Task AddFund(Fund fund);         // Changed from void
        Task UpdateFund(Fund fund);      // Changed from void
        Task DeleteFund(int id);         // Changed from void
    }

}
