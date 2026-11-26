using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IBranchReverseDashboardRepository
    {
        Task<BranchReverseSummaryViewModel> GetSummaryAsync(int branchId);
        Task<IEnumerable<DashboardDetailViewModel>> GetDetailsAsync(int branchId, string bucket);
    }
}
