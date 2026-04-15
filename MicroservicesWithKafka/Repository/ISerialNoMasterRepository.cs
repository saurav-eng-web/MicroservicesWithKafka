using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ISerialNoMasterRepository
    {
        Task<SerialNoMaster> CreateAsync(SerialNoMaster serial);
        Task<bool> UpdateAsync(SerialNoMaster serial);
        Task<bool> DeleteAsync(decimal id);
        Task<SerialNoMaster?> GetByIdAsync(decimal id);
        Task<List<SerialNoMaster>> GetAllAsync();
        Task<SerialNoMaster?> GetBySerialNumberAsync(string serialNo);
    }
}