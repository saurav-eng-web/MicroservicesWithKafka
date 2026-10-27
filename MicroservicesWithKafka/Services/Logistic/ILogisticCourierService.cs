using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticCourierService
    {
        Task<IEnumerable<CourierViewModel>> GetCouriersAsync();
        Task<(bool Success, string Message)> AddCourierAsync(CourierViewModel courier);

        // Translated from CourierMapping.aspx.vb MappingDownload_Click
        Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync();

        // Translated from CourierMapping.aspx.vb MappingNotDownload_Click
        Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync();

        // Translated from CourierMapping.aspx.vb btnUpload_Click
        Task<(bool Success, string Message)> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows);
    }
}
