using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILogisticChallanService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string? claimSfId);
    }
}
