namespace MicroservicesWithKafka.Services
{
    public interface IStockBySFService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
