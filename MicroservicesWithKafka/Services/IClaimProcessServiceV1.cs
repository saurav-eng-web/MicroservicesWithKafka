namespace MicroservicesWithKafka.Services
{
    public interface IClaimProcessServiceV1
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, int userId);
    }
}
