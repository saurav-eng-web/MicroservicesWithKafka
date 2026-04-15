namespace MicroservicesWithKafka.Repository
{
    public interface IClaimProcessRepositoryV1
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
