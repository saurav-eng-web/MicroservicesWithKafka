using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticCourierRepository
    {
        Task<IEnumerable<CourierViewModel>> GetCouriersAsync();
        Task<bool> CourierExistsAsync(string courierCode);
        Task<bool> AddCourierAsync(CourierViewModel courier);

        Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync();

        Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync();

        Task<bool> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows);
    }
}
