namespace MicroservicesWithKafka.Repository
{
    public interface IPOReceiveRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }

}
