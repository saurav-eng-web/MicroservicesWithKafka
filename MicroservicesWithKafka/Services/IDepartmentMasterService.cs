using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IDepartmentMasterService
    {
        Task<IEnumerable<DepartmentMaster>> GetAllAsync();
        Task<DepartmentMaster?> GetByIdAsync(long id);
        Task<bool> CreateAsync(DepartmentMaster model);
        Task<bool> UpdateAsync(DepartmentMaster model);
        Task<bool> DeleteAsync(long id);
    }
}
