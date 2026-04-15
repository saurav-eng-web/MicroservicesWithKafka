using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IChallanService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(ProcessChallanRequest request);
    }
}
