namespace MicroservicesWithKafka.Repository
{
    public interface ISFlevel1OperationRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
