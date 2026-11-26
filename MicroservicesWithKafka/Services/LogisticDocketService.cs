using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticDocketService : ILogisticDocketService
    {
        private const int MaxUploadRows = 50000;

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

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userId, string userName)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "list", "delete", "history", "search",
                "uploadChange", "uploadStatus", "uploadForward", "upload", "downloads", "notUploaded");

            switch (type)
            {
                case "list":
                    return (await GetDocketsAsync(LogisticPayload.Str(json, "search")), 200, "Record found Successfully");

                case "delete":
                {
                    var result = await DeleteDocketAsync(LogisticPayload.RequireInt(json, "id"));
                    return (null, result.Success ? 200 : 400, result.Message);
                }

                case "history":
                case "search":
                {
                    var challanType = ResolveChallanType(LogisticPayload.Str(json, "challanType"))
                        ?? throw new ArgumentException("A valid challanType is required (e.g. DefectivePart or 'Defective Part').");
                    var challanNo = LogisticPayload.RequireStr(json, "challanNo");

                    var detail = type == "history"
                        ? await GetDocketDetailAsync(challanType, challanNo)
                        : await SearchDocketAsync(challanType, challanNo);
                    return detail == null ? (null, 404, "No result found") : (detail, 200, "Record found Successfully");
                }

                case "uploadChange":
                {
                    var result = await UploadDocketChangeAsync(DataRows<DocketViewModel>(json), userId);
                    return (null, result.Success ? 200 : 400, result.Message);
                }

                case "uploadStatus":
                {
                    var result = await UploadDocketChangeStatusAsync(DataRows<DocketViewModel>(json));
                    return (null, result.Success ? 200 : 400, result.Message);
                }

                case "uploadForward":
                {
                    var result = await UploadForwardDocketsAsync(DataRows<ForwardDocketUploadViewModel>(json), userName);
                    return result.Success ? (result.NotApplied, 200, result.Message) : (null, 400, result.Message);
                }

                case "downloads":
                    return (await GetDocketDownloadsAsync(), 200, "Record found Successfully");

                case "notUploaded":
                    return (await GetDocketsNotUploadedAsync(), 200, "Record found Successfully");

                default:
                {
                    var result = await UploadDocketsAsync(DataRows<DocketUploadRequestViewModel>(json));
                    return (null, result.Success ? 200 : 400, result.Message);
                }
            }
        }

        private static List<T> DataRows<T>(Newtonsoft.Json.Linq.JObject json)
            => LogisticPayload.DataArray(json).ToObject<List<T>>() ?? new List<T>();

        private string? ResolveChallanType(string? type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return null;

            var key = type.Trim();
            if (ChallanTypes.TryGetValue(key, out var display))
                return display;

            var match = ChallanTypes.Values.FirstOrDefault(v => v.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;

            return ChallanTypes.TryGetValue(key.Replace(" ", string.Empty), out display) ? display : null;
        }

        private async Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search)
        {
            return await _repository.GetDocketsAsync(search);
        }

        private async Task<(bool Success, string Message)> DeleteDocketAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid docket id.");

            await _repository.DeleteDocketAsync(id);
            return (true, "Docket deleted successfully.");
        }

        private async Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo)
        {
            return await _repository.GetDocketDetailAsync(type, challanNo.Trim());
        }

        private async Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo)
        {
            return await _repository.SearchDocketAsync(type, challanNo.Trim());
        }

        private static bool IsBlank(params string?[] values) => values.All(string.IsNullOrWhiteSpace);

        private async Task<(bool Success, string Message)> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId)
        {
            if (rows == null)
                return (false, "No data provided.");
            if (string.IsNullOrWhiteSpace(userId) || userId == "0")
                return (false, "Logged-in user could not be identified.");

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

        private async Task<(bool Success, string Message)> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows)
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

        public const string FutureDateMessage = "Future Date Not Allowed Under Transaction Date Please Modify you Excel Sheet";

        private static readonly string[] TransDateFormats =
        {
            "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd",
            "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm", "dd-MM-yyyy", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "dd/MM/yyyy"
        };

        private static string? NormalizeDocStatus(string? value)
        {
            var text = (value ?? string.Empty).Trim();
            if (text.Equals("In-Transit", StringComparison.OrdinalIgnoreCase) || text.Equals("In Transit", StringComparison.OrdinalIgnoreCase))
                return "In-Transit";
            if (text.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                return "Delivered";
            return null;
        }

        private async Task<(bool Success, string Message, IReadOnlyList<ForwardDocketNotAppliedViewModel> NotApplied)> UploadForwardDocketsAsync(
            IEnumerable<ForwardDocketUploadViewModel> rows, string userName)
        {
            var none = (IReadOnlyList<ForwardDocketNotAppliedViewModel>)new List<ForwardDocketNotAppliedViewModel>();

            if (rows == null)
                return (false, "No data provided.", none);
            if (string.IsNullOrWhiteSpace(userName) || userName == "0")
                return (false, "Logged-in user could not be identified.", none);

            var list = rows.Where(r => r != null && !IsBlank(r.ChallanNo, r.DocketNo, r.DocStatus, r.TransDate)).ToList();
            if (list.Count == 0)
                return (false, "No data provided.", none);
            if (list.Count > MaxUploadRows)
                return (false, $"Too many rows. Maximum {MaxUploadRows} rows per upload.", none);

            var valid = new List<ForwardDocketRow>();
            var missing = new List<int>();
            var badStatus = new List<int>();
            var badDate = new List<int>();
            var futureDate = new List<int>();
            var today = DateTime.Today;

            for (int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                int rowNo = i + 1;
                bool ok = true;

                if (string.IsNullOrWhiteSpace(r.ChallanNo) || string.IsNullOrWhiteSpace(r.DocketNo))
                {
                    missing.Add(rowNo);
                    ok = false;
                }

                var status = NormalizeDocStatus(r.DocStatus);
                if (status == null)
                {
                    badStatus.Add(rowNo);
                    ok = false;
                }

                DateTime transDate = default;
                if (!DateTime.TryParseExact((r.TransDate ?? string.Empty).Trim(), TransDateFormats,
                        System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out transDate)
                    || transDate.Year < 1900)
                {
                    badDate.Add(rowNo);
                    ok = false;
                }
                else if (transDate.Date > today)
                {
                    futureDate.Add(rowNo);
                    ok = false;
                }

                if (ok)
                {
                    valid.Add(new ForwardDocketRow
                    {
                        ChallanNo = r.ChallanNo!.Trim(),
                        DocketNo = r.DocketNo!.Trim(),
                        DocStatus = status!,
                        TransDate = transDate,
                        UploadType = string.IsNullOrWhiteSpace(r.UploadType) ? "Forward" : r.UploadType.Trim(),
                        ChallanType = string.IsNullOrWhiteSpace(r.ChallanType) ? "Forward" : r.ChallanType.Trim()
                    });
                }
            }

            string Rows(List<int> numbers) => string.Join(", ", numbers.Take(20));

            if (futureDate.Count > 0)
                return (false, $"{FutureDateMessage}. Check row(s): {Rows(futureDate)}.", none);
            if (missing.Count > 0)
                return (false, $"ChallanNo and DocketNo are required. Check row(s): {Rows(missing)}.", none);
            if (badStatus.Count > 0)
                return (false, $"DOCStatus must be In-Transit or Delivered. Check row(s): {Rows(badStatus)}.", none);
            if (badDate.Count > 0)
                return (false, $"Invalid TransDate. Use yyyy-MM-dd or dd-MM-yyyy. Check row(s): {Rows(badDate)}.", none);

            var result = await _repository.UploadForwardDocketsAsync(valid, userName);

            var message = $"DATA IMPORTED SUCCESSFULLY. {result.Uploaded} row(s) uploaded.";
            if (result.NotApplied.Count > 0)
                message += $" {result.NotApplied.Count} row(s) could not be applied (challan / docket not found).";

            return (true, message, result.NotApplied);
        }

        private async Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync()
        {
            return await _repository.GetDocketDownloadsAsync();
        }

        private async Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync()
        {
            return await _repository.GetDocketsNotUploadedAsync();
        }

        private async Task<(bool Success, string Message)> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows)
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
