namespace MicroservicesWithKafka.Repository
{
    public interface IPOValidationRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
