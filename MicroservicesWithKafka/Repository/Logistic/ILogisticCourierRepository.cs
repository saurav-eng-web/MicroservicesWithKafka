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

        // Translated from CourierMapping.aspx.vb MappingDownload_Click
        Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync();

        // Translated from CourierMapping.aspx.vb MappingNotDownload_Click
        Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync();

        // Translated from CourierMapping.aspx.vb bindgrid — bulk upload + SP call
        Task<bool> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows);
    }
}
