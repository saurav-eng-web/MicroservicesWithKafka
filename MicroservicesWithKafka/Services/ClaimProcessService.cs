using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ClaimProcessService : IClaimProcessService
    {
        private readonly IClaimProcessRepository _repo;

        public ClaimProcessService(IClaimProcessRepository repo)
        {
            _repo = repo;
        }

        // ✅ MUST MATCH INTERFACE NAME
        public async Task<(IEnumerable<dynamic> Table1,
                           IEnumerable<dynamic> Table2,
                           int StatusCode,
                           string Message)>
            GetClaimDataAsync(ClaimProcessModel model)
        {
            return await _repo.ExecuteClaimProcessAsync(model);
        }

        public async Task<(int StatusCode, string Message)>
            UpdateClaim(ClaimProcessModel model)
        {
            return await _repo.UpdateClaimAsync(model);
        }
    }
}
