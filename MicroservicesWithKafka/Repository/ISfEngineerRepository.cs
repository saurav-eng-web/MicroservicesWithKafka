using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface ISfEngineerRepository
    {
        Task<IEnumerable<dynamic>> ManageEngineersAsync(SfEngineerRequest request);
        Task<IEnumerable<dynamic>> GetActiveEngineersBySFIDAsync(long sfid);
    }
}
