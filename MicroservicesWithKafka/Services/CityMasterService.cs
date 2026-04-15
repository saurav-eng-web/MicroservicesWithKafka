using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CityMasterService : ICityMasterService
    {
        private readonly ICityMasterRepository _cityRepo;

        public CityMasterService(ICityMasterRepository cityRepo)
        {
            _cityRepo = cityRepo;
        }

        public Task<CityMaster> CreateCityAsync(CityMaster city)
        {
            return _cityRepo.CreateCityAsync(city);
        }

        public Task<bool> UpdateCityAsync(CityMaster city)
        {
            return _cityRepo.UpdateCityAsync(city);
        }

        public Task<bool> DeleteCityAsync(decimal cityId)
        {
            return _cityRepo.DeleteCityAsync(cityId);
        }

        public Task<CityMaster?> GetCityByIdAsync(decimal cityId)
        {
            return _cityRepo.GetCityByIdAsync(cityId);
        }

        public Task<List<CityMaster>> GetAllCitiesAsync()
        {
            return _cityRepo.GetAllCitiesAsync();
        }
    }
}
