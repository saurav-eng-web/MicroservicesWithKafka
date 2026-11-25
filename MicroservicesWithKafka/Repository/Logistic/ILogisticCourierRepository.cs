using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
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
