using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IWarrantyRegistrationRepository
    {
        Task<long> InsertAsync(WarrantyRegistration model);
        Task<bool> UpdateAsync(WarrantyRegistration model);
        Task<bool> DeleteAsync(long id);
        Task<List<WarrantyRegistration>> GetAllAsync();
        Task<WarrantyRegistration?> GetByIdAsync(long id);
        Task<WarrantyRegistration?> GetBySerialNumberAsync(string serialNumber);
    }
}