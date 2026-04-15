namespace MicroservicesWithKafka.Services
{
    public interface IPOValidationService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
