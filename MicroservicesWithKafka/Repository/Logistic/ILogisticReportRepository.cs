using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>Legacy ReportLog.aspx.vb ("All Reports" in the Logistic menu). Column sets come from the SPs.</summary>
    public interface ILogisticReportRepository
    {
        Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync();
        Task<IEnumerable<dynamic>> GetUsedDocketReportAsync();
        Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync();
        Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate);
        Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate);
    }
}
