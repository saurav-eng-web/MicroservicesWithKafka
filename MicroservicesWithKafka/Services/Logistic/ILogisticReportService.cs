using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticReportService
    {
        Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync();
        Task<IEnumerable<dynamic>> GetUsedDocketReportAsync();
        Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync();
        Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate);
        Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate);
    }
}
