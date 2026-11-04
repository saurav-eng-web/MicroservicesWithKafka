using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticCourierService : ILogisticCourierService
    {
        private readonly ILogisticCourierRepository _repository;

        public LogisticCourierService(ILogisticCourierRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CourierViewModel>> GetCouriersAsync()
        {
            return await _repository.GetCouriersAsync();
        }

        public async Task<(bool Success, string Message)> AddCourierAsync(CourierViewModel courier)
        {
            if (string.IsNullOrWhiteSpace(courier.CourierCode) || string.IsNullOrWhiteSpace(courier.CourierName))
                return (false, "Courier Code and Name are required.");

            courier.CourierCode = courier.CourierCode.Trim();
            courier.CourierName = courier.CourierName.Trim();

            // Exactly replicating logic from CourierAdd.aspx.vb: Check if exists first
            bool exists = await _repository.CourierExistsAsync(courier.CourierCode);
            if (exists)
                return (false, "Courier already exists.");

            bool saved = await _repository.AddCourierAsync(courier);
            return saved ? (true, "Data saved successfully.") : (false, "Failed to save courier.");
        }

        // Translated from CourierMapping.aspx.vb MappingDownload_Click
        public async Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync()
        {
            return await _repository.GetMappedCoursAsync();
        }

        // Translated from CourierMapping.aspx.vb MappingNotDownload_Click
        public async Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync()
        {
            return await _repository.GetUnmappedCoursAsync();
        }

        // Translated from CourierMapping.aspx.vb btnUpload_Click
        public async Task<(bool Success, string Message)> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows)
        {
            if (rows == null)
                return (false, "No data provided.");

            // Excel blank rows were ignored by the legacy OLEDB import.
            var list = rows.Where(r => r != null && !(string.IsNullOrWhiteSpace(r.CourierCode) && string.IsNullOrWhiteSpace(r.Pincode)
                                                      && string.IsNullOrWhiteSpace(r.Slab) && string.IsNullOrWhiteSpace(r.Priority)
                                                      && string.IsNullOrWhiteSpace(r.Action)))
                           .ToList();
            if (list.Count == 0)
                return (false, "No data provided.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.CourierCode) || string.IsNullOrWhiteSpace(x.r.Pincode))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"CourierCode and Pincode are required. Check row(s): {string.Join(", ", missing)}.");

            foreach (var r in list)
            {
                r.CourierCode = r.CourierCode.Trim();
                r.Pincode     = r.Pincode.Trim();
                r.Slab        = r.Slab?.Trim() ?? string.Empty;
                r.Priority    = r.Priority?.Trim() ?? string.Empty;
                r.Action      = r.Action?.Trim() ?? string.Empty;
            }

            await _repository.UploadCourierMappingAsync(list);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {list.Count} row(s) uploaded.");
        }
    }
}
