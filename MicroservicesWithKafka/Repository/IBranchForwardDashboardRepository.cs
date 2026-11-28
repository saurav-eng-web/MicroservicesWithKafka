using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IBranchForwardDashboardRepository
    {
        Task<ForwardDashboardSummaryViewModel> GetSummaryAsync(DashboardScope scope);
        Task<IEnumerable<Dictionary<string, string>>> GetDetailsAsync(DashboardScope scope, string bucket);
        Task<IEnumerable<Dictionary<string, string>>> GetChallanAsync(DashboardScope scope, string challanNo);
    }
}
