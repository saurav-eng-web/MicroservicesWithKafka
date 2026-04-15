namespace MicroservicesWithKafka.Repository
{
    public interface IConsumptionRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
