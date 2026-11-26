using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticReportRepository
    {
        Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync();
        Task<IEnumerable<dynamic>> GetUsedDocketReportAsync();
        Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync();
        Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate);
        Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate);
    }
}
