using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>Legacy Mob/* pages linked from the Logistic menu ("Mobile" section).</summary>
    public interface ILogisticMobileRepository
    {
        // Mob/PrintDefectiveChallanho.aspx.vb
        Task<IEnumerable<dynamic>> GetAscListAsync();
        Task<IEnumerable<dynamic>> GetChallanListAsync(string sfId, string spType, int month, int year);

        // Mob/PrintInvoiceDef_NSP.aspx.vb
        Task<MobileChallanPrintViewModel> GetChallanPrintAsync(string poNo, string spType);

        // Mob/reportsm.aspx.vb
        Task<IEnumerable<dynamic>> GetReportAsync(string type, string fromDate, string toDate, string userId);

        // Mob/ComplaintSearch.aspx.vb / Mob/complainthistory.aspx.vb
        Task<IEnumerable<dynamic>> SearchComplaintsAsync(string text, string userId, string ipAddress);
        Task<MobileComplaintHistoryViewModel> GetComplaintHistoryAsync(string ticketKey, string userId);
    }
}
