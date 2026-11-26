using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticMobileRepository
    {
        Task<IEnumerable<dynamic>> GetAscListAsync();
        Task<IEnumerable<dynamic>> GetChallanListAsync(string sfId, string spType, int month, int year);

        Task<MobileChallanPrintViewModel> GetChallanPrintAsync(string poNo, string spType);

        Task<IEnumerable<dynamic>> GetReportAsync(string type, string fromDate, string toDate, string userId);

        Task<IEnumerable<dynamic>> SearchComplaintsAsync(string text, string userId, string ipAddress);
        Task<MobileComplaintHistoryViewModel> GetComplaintHistoryAsync(string ticketKey, string userId);
    }
}
