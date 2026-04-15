using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class MenuMasterService : IMenuMasterService
    {
        private readonly IMenuMasterRepository _repo;

        public MenuMasterService(IMenuMasterRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(MenuMasterRequest model)
        {
            return await _repo.ExecuteAsync(model);
        }
    }
}
