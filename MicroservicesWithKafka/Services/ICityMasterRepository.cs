using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ICityMasterRepository
    {
        Task<CityMaster> CreateCityAsync(CityMaster city);
        Task<bool> UpdateCityAsync(CityMaster city);
        Task<bool> DeleteCityAsync(decimal cityId);
        Task<CityMaster?> GetCityByIdAsync(decimal cityId);
        Task<List<CityMaster>> GetAllCitiesAsync();
    }
}
