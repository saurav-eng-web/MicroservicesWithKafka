using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IRoleMasterService
    {
        Task<RoleMaster?> GetRoleByIdAsync(int id);
        Task<List<RoleMaster>> GetAllRolesAsync();
        Task<RoleMaster> CreateRoleAsync(RoleMaster model);
        Task<bool> UpdateRoleAsync(RoleMaster model);
        Task<bool> DeleteRoleAsync(int id);
    }

}
