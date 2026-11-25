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

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            LogisticPayload.RequireType(json, "summary");

            var data = await _repository.GetDashboardSummaryAsync();
            return (data, 200, "Record found Successfully");
        }
    }
}
