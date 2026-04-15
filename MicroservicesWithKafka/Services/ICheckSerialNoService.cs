namespace MicroservicesWithKafka.Services
{
    public interface ICheckSerialNoService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload);
    }
}
