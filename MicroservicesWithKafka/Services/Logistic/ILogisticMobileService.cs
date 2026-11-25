using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticMobileService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userId, string ipAddress);
    }
}
