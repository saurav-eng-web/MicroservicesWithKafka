using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class AscPendingChallanService : IAscPendingChallanService
    {
        private readonly IAscPendingChallanRepository _repo;

        public AscPendingChallanService(IAscPendingChallanRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(int id, string type)
        {
            return await _repo.ExecuteAsync(id, type);
        }
    }
}
