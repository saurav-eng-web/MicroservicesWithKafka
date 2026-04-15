using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class RegionMasterService : IRegionMasterService
    {
        private readonly IRegionMasterRepository _repository;

        public RegionMasterService(IRegionMasterRepository repository)
        {
            _repository = repository;
        }

        public Task<RegionMaster> AddRegionAsync(RegionMaster region) => _repository.CreateRegionAsync(region);
        public Task<bool> UpdateRegionAsync(RegionMaster region) => _repository.UpdateRegionAsync(region);
        public Task<bool> DeleteRegionAsync(decimal regionId) => _repository.DeleteRegionAsync(regionId);
        public Task<RegionMaster?> GetRegionByIdAsync(decimal regionId) => _repository.GetRegionByIdAsync(regionId);
        public Task<List<RegionMaster>> GetAllRegionsAsync() => _repository.GetAllRegionsAsync();
    }
}
