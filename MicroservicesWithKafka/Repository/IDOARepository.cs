namespace MicroservicesWithKafka.Repository
{
    public interface IDOARepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
