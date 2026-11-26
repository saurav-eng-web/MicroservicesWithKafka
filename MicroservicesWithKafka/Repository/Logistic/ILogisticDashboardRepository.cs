using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticDashboardRepository
    {
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync();
        Task<IEnumerable<DashboardDetailViewModel>> GetDashboardDetailsAsync(string bucket);
    }
}
