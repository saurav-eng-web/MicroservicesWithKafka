using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IRoleMasterRepository
    {
        Task<RoleMaster?> GetRoleByIdAsync(int id);
        Task<List<RoleMaster>> GetAllRolesAsync();
        Task<RoleMaster> CreateRoleAsync(RoleMaster model);
        Task<bool> UpdateRoleAsync(RoleMaster model);
        Task<bool> DeleteRoleAsync(int id);
    }
}
