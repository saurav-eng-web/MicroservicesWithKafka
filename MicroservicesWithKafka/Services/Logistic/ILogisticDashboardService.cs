using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticDashboardService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload);
    }
}
