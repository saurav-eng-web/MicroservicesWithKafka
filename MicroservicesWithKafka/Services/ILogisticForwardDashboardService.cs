using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILogisticForwardDashboardService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload);
    }
}
