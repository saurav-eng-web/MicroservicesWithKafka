using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticDocketService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userId, string userName);
    }
}
