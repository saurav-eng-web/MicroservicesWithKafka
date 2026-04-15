namespace MicroservicesWithKafka.Services
{
    public interface ISymptomCauseService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object payload, string sfId);
    }
}
