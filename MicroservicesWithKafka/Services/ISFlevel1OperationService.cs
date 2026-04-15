namespace MicroservicesWithKafka.Services
{
    public interface ISFlevel1OperationService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
