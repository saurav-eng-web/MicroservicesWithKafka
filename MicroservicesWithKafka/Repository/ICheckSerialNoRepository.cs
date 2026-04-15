namespace MicroservicesWithKafka.Repository
{
    public interface ICheckSerialNoRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
