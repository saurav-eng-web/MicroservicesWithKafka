using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IBranchReverseDashboardRepository
    {
        Task<BranchReverseSummaryViewModel> GetSummaryAsync(DashboardScope scope);
        Task<IEnumerable<Dictionary<string, string>>> GetDetailsAsync(DashboardScope scope, string bucket);
    }
}
