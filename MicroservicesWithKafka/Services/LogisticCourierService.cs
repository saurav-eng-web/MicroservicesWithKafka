using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticCourierService : ILogisticCourierService
    {
        private readonly ILogisticCourierRepository _repository;

        public LogisticCourierService(ILogisticCourierRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "list", "add", "mapped", "unmapped", "uploadMapping");

            switch (type)
            {
                case "list":
                    return (await _repository.GetCouriersAsync(), 200, "Record found Successfully");

                case "add":
                    return await AddCourierAsync(json);

                case "mapped":
                    return (await _repository.GetMappedCoursAsync(), 200, "Record found Successfully");

                case "unmapped":
                    return (await _repository.GetUnmappedCoursAsync(), 200, "Record found Successfully");

                default:
                    return await UploadCourierMappingAsync(json);
            }
        }

        private async Task<(object? Data, int StatusCode, string StatusMsg)> AddCourierAsync(JObject json)
        {
            var courier = new CourierViewModel
            {
                CourierCode = LogisticPayload.Str(json, "courierCode"),
                CourierName = LogisticPayload.Str(json, "courierName")
            };

            if (courier.CourierCode.Length == 0 || courier.CourierName.Length == 0)
                return (null, 400, "Courier Code and Name are required.");

            if (await _repository.CourierExistsAsync(courier.CourierCode))
                return (null, 400, "Courier already exists.");

            bool saved = await _repository.AddCourierAsync(courier);
            return saved ? (null, 200, "Data saved successfully.") : (null, 400, "Failed to save courier.");
        }

        private async Task<(object? Data, int StatusCode, string StatusMsg)> UploadCourierMappingAsync(JObject json)
        {
            var rows = LogisticPayload.DataArray(json).ToObject<List<CourierMappingUploadViewModel>>() ?? new List<CourierMappingUploadViewModel>();

            var list = rows.Where(r => r != null && !(string.IsNullOrWhiteSpace(r.CourierCode) && string.IsNullOrWhiteSpace(r.Pincode)
                                                      && string.IsNullOrWhiteSpace(r.Slab) && string.IsNullOrWhiteSpace(r.Priority)
                                                      && string.IsNullOrWhiteSpace(r.Action)))
                           .ToList();
            if (list.Count == 0)
                return (null, 400, "No data provided.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.CourierCode) || string.IsNullOrWhiteSpace(x.r.Pincode))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (null, 400, $"CourierCode and Pincode are required. Check row(s): {string.Join(", ", missing)}.");

            foreach (var r in list)
            {
                r.CourierCode = r.CourierCode.Trim();
                r.Pincode     = r.Pincode.Trim();
                r.Slab        = r.Slab?.Trim() ?? string.Empty;
                r.Priority    = r.Priority?.Trim() ?? string.Empty;
                r.Action      = r.Action?.Trim() ?? string.Empty;
            }

            await _repository.UploadCourierMappingAsync(list);
            return (null, 200, $"DATA IMPORTED SUCCESSFULLY. {list.Count} row(s) uploaded.");
        }
    }
}
