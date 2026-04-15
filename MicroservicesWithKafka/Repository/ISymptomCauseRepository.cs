namespace MicroservicesWithKafka.Repository
{
    public interface ISymptomCauseRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request);
    }
}
