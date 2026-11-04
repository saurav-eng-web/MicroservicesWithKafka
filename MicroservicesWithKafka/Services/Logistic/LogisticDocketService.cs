using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticDocketService : ILogisticDocketService
    {
        private const int MaxUploadRows = 50000;

        // Legacy StatusView.aspx.vb stored lblHeader.Text (the display name) in Session("DhamageShortName")
        // and History.aspx.vb passed it to sp_DocketDetails @Type. SearchHistory.aspx used the same
        // display names in its dropdown. StatusView/dashboard links use the codes on the left.
        private static readonly Dictionary<string, string> ChallanTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["DefectivePart"]   = "Defective Part",
            ["DefectiveDOA"]    = "Defective DOA",
            ["GoodPart"]        = "Good Part",
            ["GoodMachine"]     = "Good Machine",
            ["MismatchPart"]    = "Mismatch Part",
            ["MismatchMachine"] = "Mismatch Machine"
        };

        private readonly ILogisticDocketRepository _repository;

        public LogisticDocketService(ILogisticDocketRepository repository)
        {
            _repository = repository;
        }

        public string? ResolveChallanType(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var key = type.Trim();
            if (ChallanTypes.TryGetValue(key, out var display))
                return display;

            // Already a display name ("Defective Part")?
            var match = ChallanTypes.Values.FirstOrDefault(v => v.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;

            // Tolerate "Defective%20Part" variants without spaces, e.g. "defective part" -> code lookup
            return ChallanTypes.TryGetValue(key.Replace(" ", string.Empty), out display) ? display : null;
        }

        public async Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search)
        {
            return await _repository.GetDocketsAsync(search);
        }

        public async Task<(bool Success, string Message)> DeleteDocketAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid docket id.");

            await _repository.DeleteDocketAsync(id);
            return (true, "Docket deleted successfully.");
        }

        public async Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo)
        {
            return await _repository.GetDocketDetailAsync(type, challanNo.Trim());
        }

        public async Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo)
        {
            return await _repository.SearchDocketAsync(type, challanNo.Trim());
        }

        // ---------------- Uploads ----------------

        private static bool IsBlank(params string?[] values) => values.All(string.IsNullOrWhiteSpace);

        public async Task<(bool Success, string Message)> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId)
        {
            if (rows == null)
                return (false, "No data provided.");
            if (string.IsNullOrWhiteSpace(userId) || userId == "0")
                return (false, "Logged-in user could not be identified.");

            // Excel blank rows were ignored by the legacy import; skip them here as well.
            var list = rows.Where(r => r != null && !IsBlank(r.ChallanNo, r.CourierCode, r.DocketNo, r.Type)).ToList();
            if (list.Count == 0)
                return (false, "No data provided.");
            if (list.Count > MaxUploadRows)
                return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.ChallanNo) || string.IsNullOrWhiteSpace(x.r.DocketNo))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"ChallanNo and DocketNo are required. Check row(s): {string.Join(", ", missing)}.");

            int count = await _repository.UploadDocketChangeAsync(list, userId);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }

        public async Task<(bool Success, string Message)> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows)
        {
            if (rows == null)
                return (false, "No data provided.");

            var list = rows.Where(r => r != null && !IsBlank(r.ChallanNo, r.Type, r.PickupDate, r.DeliveryDate, r.DocStatus)).ToList();
            if (list.Count == 0)
                return (false, "No data provided.");
            if (list.Count > MaxUploadRows)
                return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.ChallanNo))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"ChallanNo is required. Check row(s): {string.Join(", ", missing)}.");

            // Validate dates before touching the database so a bad row never leaves a partial import.
            var badDates = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    LogisticDocketRepository.ParseDate(list[i].PickupDate);
                    LogisticDocketRepository.ParseDate(list[i].DeliveryDate);
                }
                catch (FormatException)
                {
                    badDates.Add(i + 1);
                    if (badDates.Count >= 20) break;
                }
            }
            if (badDates.Count > 0)
                return (false, $"Invalid PickupDate/DeliveryDate. Use yyyy-MM-dd or dd-MM-yyyy. Check row(s): {string.Join(", ", badDates)}.");

            int count = await _repository.UploadDocketChangeStatusAsync(list);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }

        public async Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync()
        {
            return await _repository.GetDocketDownloadsAsync();
        }

        public async Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync()
        {
            return await _repository.GetDocketsNotUploadedAsync();
        }

        public async Task<(bool Success, string Message)> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows)
        {
            if (rows == null)
                return (false, "No data provided.");

            var list = rows.Where(r => r != null && !IsBlank(r.CourierCode, r.DocketNo)).ToList();
            if (list.Count == 0)
                return (false, "No data provided.");
            if (list.Count > MaxUploadRows)
                return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.");

            var missing = list.Select((r, i) => new { r, Row = i + 1 })
                              .Where(x => string.IsNullOrWhiteSpace(x.r.CourierCode) || string.IsNullOrWhiteSpace(x.r.DocketNo))
                              .Select(x => x.Row).Take(20).ToList();
            if (missing.Count > 0)
                return (false, $"CourierCode and DocketNo are required. Check row(s): {string.Join(", ", missing)}.");

            int count = await _repository.UploadDocketsAsync(list);
            return (true, $"DATA IMPORTED SUCCESSFULLY. {count} row(s) uploaded.");
        }
    }
}
