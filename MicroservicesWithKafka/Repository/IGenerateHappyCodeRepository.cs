namespace MicroservicesWithKafka.Repository
{
    public interface IGenerateHappyCodeRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
