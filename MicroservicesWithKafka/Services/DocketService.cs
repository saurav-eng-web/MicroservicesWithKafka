using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class DocketService : IDocketService
    {
        private readonly IDocketRepository _repo;

        public DocketService(IDocketRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetDocketAsync(object request)
        {
            return await _repo.GetDocketAsync(request);
        }
    }
}
