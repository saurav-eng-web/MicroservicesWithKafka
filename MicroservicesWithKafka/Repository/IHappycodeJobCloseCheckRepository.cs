namespace MicroservicesWithKafka.Repository
{
    public interface IHappycodeJobCloseCheckRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
