using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticMobileService : ILogisticMobileService
    {
        // Legacy Mob/reportsm.aspx "Select Report Name" dropdown values (passed to reportsm @type).
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

        public bool IsValidReportType(string? type) => type != null && ReportTypes.Contains(type);

        public Task<IEnumerable<dynamic>> GetAscListAsync() => _repository.GetAscListAsync();

        public Task<IEnumerable<dynamic>> GetChallanListAsync(string sfId, string spType, int month, int year)
            => _repository.GetChallanListAsync(sfId, spType, month, year);

        public Task<MobileChallanPrintViewModel> GetChallanPrintAsync(string poNo, string spType)
            => _repository.GetChallanPrintAsync(poNo.Trim(), spType);

        public Task<IEnumerable<dynamic>> GetReportAsync(string type, string fromDate, string toDate, string userId)
            => _repository.GetReportAsync(type, fromDate, toDate, userId);

        public Task<IEnumerable<dynamic>> SearchComplaintsAsync(string text, string userId, string ipAddress)
            => _repository.SearchComplaintsAsync(text.Trim(), userId, ipAddress);

        public Task<MobileComplaintHistoryViewModel> GetComplaintHistoryAsync(string ticketKey, string userId)
            => _repository.GetComplaintHistoryAsync(ticketKey.Trim(), userId);
    }
}
