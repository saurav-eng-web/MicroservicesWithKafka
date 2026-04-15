namespace MicroservicesWithKafka.Services
{
    public interface IHappycodeJobCloseCheckService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload);
    }
}
