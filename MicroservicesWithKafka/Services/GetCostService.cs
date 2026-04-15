using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class GetCostService : IGetCostService
    {
        private readonly IGetCostRepository _repo;

        public GetCostService(IGetCostRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object request)
        {
            return await _repo.ExecuteAsync(request);
        }
    }
}
