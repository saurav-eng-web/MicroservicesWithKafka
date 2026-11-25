using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticReportService : ILogisticReportService
    {
        private static readonly string[] Reports =
            { "available-docket", "used-docket", "complete-docket", "total-dispatch", "total-dispatch-header" };

        private readonly ILogisticReportRepository _repository;

        public LogisticReportService(ILogisticReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "report", "download");
            var report = Str(json, "report");
            if (!Reports.Contains(report, StringComparer.Ordinal))
                throw new ArgumentException("report must be one of " + string.Join(", ", Reports) + ".");

            string reportName;
            Func<Task<IEnumerable<dynamic>>> query;

            switch (report)
            {
                case "available-docket":
                    reportName = "Available Docket Report";
                    query = _repository.GetAvailableDocketReportAsync;
                    break;
                case "used-docket":
                    reportName = "Used Docket Report";
                    query = _repository.GetUsedDocketReportAsync;
                    break;
                case "complete-docket":
                    reportName = "Complete Docket Report";
                    query = _repository.GetCompleteDocketReportAsync;
                    break;
                case "total-dispatch":
                {
                    var (from, to) = GetRange(Str(json, "fromDate"), Str(json, "toDate"));
                    reportName = type == "download" ? "Dispatch Line Report" : "Total Dispatch Report";
                    query = () => _repository.GetTotalDispatchReportAsync(from, to);
                    break;
                }
                default:
                {
                    var (from, to) = GetRange(Str(json, "fromDate"), Str(json, "toDate"));
                    reportName = type == "download" ? "Dispatch Header Report" : "Total Dispatch Header Report";
                    query = () => _repository.GetTotalDispatchHeaderReportAsync(from, to);
                    break;
                }
            }

            var rows = await query();

            if (type == "report")
                return (rows, 200, "Record found Successfully");

            return (new LogisticReportFile
            {
                ReportName = reportName,
                Rows = rows.Select(r => (IDictionary<string, object>)r).ToList()
            }, 200, "Record found Successfully");
        }

        private static string Str(Newtonsoft.Json.Linq.JObject json, string key) => LogisticPayload.Str(json, key);

        private static (string From, string To) GetRange(string fromDate, string toDate)
        {
            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" };

            if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                throw new ArgumentException("fromDate and toDate are required.");
            if (!DateTime.TryParseExact(fromDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) ||
                !DateTime.TryParseExact(toDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                throw new ArgumentException("Dates must be in yyyy-MM-dd format.");
            if (f > t)
                throw new ArgumentException("fromDate cannot be after toDate.");

            return (f.ToString("yyyy-MM-dd"), t.ToString("yyyy-MM-dd"));
        }
    }
}
