namespace MicroservicesWithKafka.Services
{
    public interface IGenerateHappyCodeService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string userName);
    }
}
