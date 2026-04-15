using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IWarrantyMasterRepository
    {
        Task<IEnumerable<WarrantyMaster>> GetAllAsync();
        Task<WarrantyMaster?> GetByIdAsync(long id);
        Task<long> AddAsync(WarrantyMaster warranty);
        Task<long> UpdateAsync(WarrantyMaster warranty);
        Task<long> DeleteAsync(long id);
    }
}
