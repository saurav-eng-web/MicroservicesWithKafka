namespace MicroservicesWithKafka.Repository
{
    public interface IOKCallRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
