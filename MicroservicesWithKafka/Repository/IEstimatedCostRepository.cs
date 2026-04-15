namespace MicroservicesWithKafka.Repository
{
    public interface IEstimatedCostRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
             ExecuteAsync(object request);
    }
}
