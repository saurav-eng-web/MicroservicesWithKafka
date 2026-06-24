namespace MicroservicesWithKafka.Repository
{
    public interface IFinanceClaimCountRepository
    {
        // Type-dispatched Finance proc SP_CRUD_GetFinanceClaimCounts: each data result set is returned
        // separately as DataSets (trailing { StatusCode, StatusMsg } stripped). 'nsp' yields 2 datasets.
        Task<(List<List<dynamic>> DataSets, int StatusCode, string StatusMsg)> ExecuteAsync(object request);
    }
}
