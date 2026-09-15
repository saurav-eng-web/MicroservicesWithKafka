namespace MicroservicesWithKafka.Repository
{
    public interface ITDDOAPickupRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
