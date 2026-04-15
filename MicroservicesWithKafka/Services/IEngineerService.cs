using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IEngineerService
    {
        Task<IEnumerable<dynamic>> ManageEngineersAsync(SfEngineerRequest request);
        Task<IEnumerable<dynamic>> GetActiveEngineersBySFIDAsync(long sfid);
    }
}
