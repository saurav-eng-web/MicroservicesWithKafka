using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IRegionMasterService
    {
        Task<RegionMaster> AddRegionAsync(RegionMaster region);
        Task<bool> UpdateRegionAsync(RegionMaster region);
        Task<bool> DeleteRegionAsync(decimal regionId);
        Task<RegionMaster?> GetRegionByIdAsync(decimal regionId);
        Task<List<RegionMaster>> GetAllRegionsAsync();
    }
}
