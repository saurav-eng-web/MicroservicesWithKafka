using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticInvoiceUploadService : ILogisticInvoiceUploadService
    {
        private const int MaxUploadRows = 50000;
        private readonly ILogisticInvoiceUploadRepository _repository;

        public LogisticInvoiceUploadService(ILogisticInvoiceUploadRepository repository)
        {
            _repository = repository;
        }

        private static bool AllBlank(params string?[] values) => values.All(string.IsNullOrWhiteSpace);

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userName)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "po", "rate");
            var data = LogisticPayload.DataArray(json);

            var result = type == "po"
                ? await UploadInvoicePoAsync(data.ToObject<List<LogisticInvoicePoUploadViewModel>>() ?? new(), userName)
                : await UploadInvoiceRateAsync(data.ToObject<List<LogisticInvoiceRateUploadViewModel>>() ?? new(), userName);

            return (null, result.Success ? 200 : 400, result.Message);
        }

        private async Task<(bool Success, string Message)> UploadInvoicePoAsync(IEnumerable<LogisticInvoicePoUploadViewModel> rows, string userName)
        {
            if (rows == null) return (false, "No data provided.");
            if (string.IsNullOrWhiteSpace(userName)) return (false, "Logged-in user could not be identified.");

            var list = rows.Where(r => r != null && !AllBlank(r.InvoiceNo, r.FromA, r.Shipto, r.TransporterName, r.DocketNo, r.VehicleType)).ToList();
            if (list.Count == 0) return (false, "No data provided.");
            if (list.Count > MaxUploadRows) return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.InvoiceNo))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"InvoiceNo is required. Check row(s): {string.Join(", ", missing)}.");

            int count = await _repository.UploadInvoicePoAsync(list, userName);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }

        private async Task<(bool Success, string Message)> UploadInvoiceRateAsync(IEnumerable<LogisticInvoiceRateUploadViewModel> rows, string userName)
        {
            if (rows == null) return (false, "No data provided.");
            if (string.IsNullOrWhiteSpace(userName)) return (false, "Logged-in user could not be identified.");

            var list = rows.Where(r => r != null && !AllBlank(r.FromA, r.ToA, r.Transporter, r.VehicleType, r.Rate)).ToList();
            if (list.Count == 0) return (false, "No data provided.");
            if (list.Count > MaxUploadRows) return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.FromA) || string.IsNullOrWhiteSpace(x.r.ToA))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"FromA and ToA are required. Check row(s): {string.Join(", ", missing)}.");

            var badRate = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => !string.IsNullOrWhiteSpace(x.r.Rate)
                                          && !decimal.TryParse(x.r.Rate.Trim(), System.Globalization.NumberStyles.Number,
                                                               System.Globalization.CultureInfo.InvariantCulture, out _))
                              .Select(x => x.Row).Take(20).ToList();
            if (badRate.Count > 0)
                return (false, $"Rate must be a number. Check row(s): {string.Join(", ", badRate)}.");

            int count = await _repository.UploadInvoiceRateAsync(list, userName);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }
    }
}
