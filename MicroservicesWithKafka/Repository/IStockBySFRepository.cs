namespace MicroservicesWithKafka.Repository
{
    public interface IStockBySFRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
