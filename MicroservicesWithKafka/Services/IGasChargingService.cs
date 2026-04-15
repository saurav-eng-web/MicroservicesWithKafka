namespace MicroservicesWithKafka.Services
{
    public interface IGasChargingService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
