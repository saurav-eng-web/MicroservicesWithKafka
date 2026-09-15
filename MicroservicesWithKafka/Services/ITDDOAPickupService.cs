namespace MicroservicesWithKafka.Services
{
    public interface ITDDOAPickupService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string userId);
    }
}
