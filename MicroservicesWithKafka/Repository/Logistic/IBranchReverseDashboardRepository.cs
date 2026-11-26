using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface IBranchReverseDashboardRepository
    {
        Task<BranchReverseSummaryViewModel> GetSummaryAsync(int branchId);
        Task<IEnumerable<DashboardDetailViewModel>> GetDetailsAsync(int branchId, string bucket);
    }
}
