using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class EngineerService : IEngineerService
    {
        private readonly ISfEngineerRepository _repo;

        public EngineerService(ISfEngineerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<dynamic>> ManageEngineersAsync(SfEngineerRequest request)
        {
            return await _repo.ManageEngineersAsync(request);
        }

        public async Task<IEnumerable<dynamic>> GetActiveEngineersBySFIDAsync(long sfid)
        {
            return await _repo.GetActiveEngineersBySFIDAsync(sfid);
        }
    }
}
