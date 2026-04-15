using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILocalityMasterService
    {
        Task<LocalityMaster> CreateLocalityAsync(LocalityMaster model);
        Task<bool> UpdateLocalityAsync(LocalityMaster model);
        Task<bool> DeleteLocalityAsync(long id);
        Task<LocalityMaster?> GetLocalityByIdAsync(long id);
        Task<List<LocalityMaster>> GetAllLocalitiesAsync();
        Task<List<LocalityMaster>> GetLocalitiesByPincodeAsync(string pincode);
    }
}
