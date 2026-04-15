namespace MicroservicesWithKafka.Services
{
    public interface IPOReceiveService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }

}
