using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticDashboardService : ILogisticDashboardService
    {
        private readonly ILogisticDashboardRepository _repository;

        public LogisticDashboardService(ILogisticDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            return await _repository.GetDashboardSummaryAsync();
        }
    }
}
