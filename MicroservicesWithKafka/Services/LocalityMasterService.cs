using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LocalityMasterService : ILocalityMasterService
    {
        private readonly ILocalityMasterRepository _repository;

        public LocalityMasterService(ILocalityMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<LocalityMaster> CreateLocalityAsync(LocalityMaster model)
        {
            return await _repository.CreateLocalityAsync(model);
        }

        public async Task<bool> UpdateLocalityAsync(LocalityMaster model)
        {
            return await _repository.UpdateLocalityAsync(model);
        }

        public async Task<bool> DeleteLocalityAsync(long id)
        {
            return await _repository.DeleteLocalityAsync(id);
        }

        public async Task<LocalityMaster?> GetLocalityByIdAsync(long id)
        {
            return await _repository.GetLocalityByIdAsync(id);
        }


        public async Task<List<LocalityMaster>> GetLocalitiesByPincodeAsync(string pincode)
        {
            return await _repository.GetLocalitiesByPincodeAsync(pincode);
        }
        public async Task<List<LocalityMaster>> GetAllLocalitiesAsync()
        {
            return await _repository.GetAllLocalitiesAsync();
        }
    }
}
