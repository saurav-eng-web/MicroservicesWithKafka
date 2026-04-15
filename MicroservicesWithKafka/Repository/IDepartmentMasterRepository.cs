using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IDepartmentMasterRepository
    {
        Task<IEnumerable<DepartmentMaster>> GetAllAsync();
        Task<DepartmentMaster?> GetByIdAsync(long id);
        Task<bool> CreateAsync(DepartmentMaster model);
        Task<bool> UpdateAsync(DepartmentMaster model);
        Task<bool> DeleteAsync(long id);
    }
}
