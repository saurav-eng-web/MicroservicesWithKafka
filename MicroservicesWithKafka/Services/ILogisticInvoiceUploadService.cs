using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILogisticInvoiceUploadService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userName);
    }
}
