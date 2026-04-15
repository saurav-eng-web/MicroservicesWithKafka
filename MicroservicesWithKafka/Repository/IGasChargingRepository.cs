namespace MicroservicesWithKafka.Repository
{
    public interface IGasChargingRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
