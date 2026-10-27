using MicroservicesWithKafka.Models.Logistic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticDashboardRepository
    {
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync();
    }
}
