using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticChallanService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string? claimSfId);
    }
}
