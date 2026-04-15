using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ISerialNoMasterService
    {
        Task<SerialNoMaster> CreateSerialNoAsync(SerialNoMaster serial);
        Task<bool> UpdateSerialNoAsync(SerialNoMaster serial);
        Task<bool> DeleteSerialNoAsync(decimal id);
        Task<SerialNoMaster?> GetSerialNoByIdAsync(decimal id);
        Task<List<SerialNoMaster>> GetAllSerialNosAsync();
        Task<SerialNoMaster?> GetBySerialNumberAsync(string serialNo);
    }
}
