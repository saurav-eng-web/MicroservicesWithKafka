namespace MicroservicesWithKafka.Repository
{
    public interface ICancellationRequestRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
