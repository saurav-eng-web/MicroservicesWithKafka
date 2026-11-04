using MicroservicesWithKafka.Models.Logistic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticDashboardService
    {
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync();
    }
}
