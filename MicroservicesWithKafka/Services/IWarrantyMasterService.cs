using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IWarrantyMasterService
    {
        Task<IEnumerable<WarrantyMaster>> GetAllAsync();
        Task<WarrantyMaster?> GetByIdAsync(long id);
        Task<long> AddAsync(WarrantyMaster warranty);
        Task<long> UpdateAsync(WarrantyMaster warranty);
        Task<long> DeleteAsync(long id);
    }
}
