using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class RoleMasterService : IRoleMasterService
    {
        private readonly IRoleMasterRepository _repo;

        public RoleMasterService(IRoleMasterRepository repo)
        {
            _repo = repo;
        }

        public Task<RoleMaster> CreateRoleAsync(RoleMaster model) => _repo.CreateRoleAsync(model);
        public Task<bool> DeleteRoleAsync(int id) => _repo.DeleteRoleAsync(id);
        public Task<List<RoleMaster>> GetAllRolesAsync() => _repo.GetAllRolesAsync();
        public Task<RoleMaster?> GetRoleByIdAsync(int id) => _repo.GetRoleByIdAsync(id);
        public Task<bool> UpdateRoleAsync(RoleMaster model) => _repo.UpdateRoleAsync(model);
    }

}
