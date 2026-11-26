using MicroservicesWithKafka.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticMobileService : ILogisticMobileService
    {
        private static readonly HashSet<string> ReportTypes = new(StringComparer.Ordinal)
        {
            "OpencallReport", "JobsheetMaster", "ReverseLogistic", "partconsumption", "STNreport", "Inventory",
            "Gooddefectivereturn", "BOMReport", "PartPriceList", "LogisticRevRep", "LogisticFwdRep",
            "PendingDispatchSp", "ASCMaster", "Claim", "ClaimDetail"
        };

        private readonly ILogisticMobileRepository _repository;

        public LogisticMobileService(ILogisticMobileRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userId, string ipAddress)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "asclist", "challans", "print", "report", "complaintSearch", "complaintHistory");

            switch (type)
            {
                case "asclist":
                    return (await _repository.GetAscListAsync(), 200, "Record found Successfully");

                case "challans":
                {
                    var sfId = LogisticPayload.Str(json, "sfId");
                    if (sfId.Length == 0)
                        throw new ArgumentException("sfId is required (select a service center).");
                    var month = LogisticPayload.RequireInt(json, "month");
                    var year = LogisticPayload.RequireInt(json, "year");
                    if (month < 1 || month > 12)
                        throw new ArgumentException("month must be between 1 and 12.");
                    if (year < 2000 || year > DateTime.Now.Year + 1)
                        throw new ArgumentException("year is invalid.");
                    var spType = LogisticChallanService.ResolvePrintChallanType(LogisticPayload.Str(json, "challanType"))
                        ?? throw new ArgumentException("challanType must be one of Defective, DOA, GoodSpare, GoodFG, MisFG, MisSpare.");

                    return (await _repository.GetChallanListAsync(sfId, spType, month, year), 200, "Record found Successfully");
                }

                case "print":
                {
                    var poNo = LogisticPayload.RequireStr(json, "poNo");
                    var spType = LogisticChallanService.ResolvePrintChallanType(LogisticPayload.Str(json, "challanType"))
                        ?? throw new ArgumentException("challanType is invalid.");

                    var result = await _repository.GetChallanPrintAsync(poNo, spType);
                    if (result.Header == null && !result.Lines.Any())
                        return (null, 404, "No result found");
                    return (result, 200, "Record found Successfully");
                }

                case "report":
                {
                    var reportType = LogisticPayload.Str(json, "reportType");
                    if (!ReportTypes.Contains(reportType))
                        throw new ArgumentException("Please select a valid report.");

                    string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" };
                    if (!DateTime.TryParseExact(LogisticPayload.Str(json, "fromDate"), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var from) ||
                        !DateTime.TryParseExact(LogisticPayload.Str(json, "toDate"), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var to))
                        throw new ArgumentException("From Date and To Date are required (yyyy-MM-dd).");
                    if (from > to)
                        throw new ArgumentException("From Date cannot be after To Date.");

                    var rows = await _repository.GetReportAsync(reportType, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"), userId);
                    return (new LogisticReportFile
                    {
                        ReportName = $"{reportType}_{DateTime.Now:yyyyMMddHHmmss}",
                        Rows = rows.Select(r => (IDictionary<string, object>)r).ToList()
                    }, 200, "Record found Successfully");
                }

                case "complaintSearch":
                {
                    var text = LogisticPayload.Str(json, "text");
                    if (text.Length == 0)
                        throw new ArgumentException("Please enter a search value.");
                    return (await _repository.SearchComplaintsAsync(text, userId, ipAddress), 200, "Record found Successfully");
                }

                default:
                {
                    var ticketKey = LogisticPayload.RequireStr(json, "ticketKey");
                    return (await _repository.GetComplaintHistoryAsync(ticketKey, userId), 200, "Record found Successfully");
                }
            }
        }
    }
}
