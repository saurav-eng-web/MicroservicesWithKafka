using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IWarrantyRegistrationService
    {
        Task<long> AddAsync(WarrantyRegistration model);
        Task<bool> UpdateAsync(WarrantyRegistration model);
        Task<bool> DeleteAsync(long id);
        Task<List<WarrantyRegistration>> GetAllAsync();
        Task<WarrantyRegistration?> GetByIdAsync(long id);
        Task<WarrantyRegistration?> GetBySerialNumberAsync(string serialNumber);
    }
}
