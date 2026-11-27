using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticDashboardRepository
    {
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync();
        Task<IEnumerable<Dictionary<string, string>>> GetDashboardDetailsAsync(string bucket);
    }
}
