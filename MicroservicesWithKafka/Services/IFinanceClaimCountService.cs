namespace MicroservicesWithKafka.Services
{
    public interface IFinanceClaimCountService
    {
        // Finance operations -> SP_CRUD_GetFinanceClaimCounts. Dynamic payload (caller sends $.type); userId from JWT.
        // Returns each result set separately as DataSets (e.g. 'nsp' = summary + top-10 ASC).
        Task<(List<List<dynamic>> DataSets, int StatusCode, string StatusMsg)> GetClaimCountsAsync(object payload, int userId);
    }
}
