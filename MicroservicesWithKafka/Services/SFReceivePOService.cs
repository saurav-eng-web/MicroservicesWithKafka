using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class SFReceivePOService : ISFReceivePOService
    {
        private readonly ISFReceivePORepository _repo;

        public SFReceivePOService(ISFReceivePORepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<dynamic>> ManageSFReceivePOAsync(SFReceivePORequest request)
        {
            return await _repo.ManageSFReceivePOAsync(request);
        }
    }
}
