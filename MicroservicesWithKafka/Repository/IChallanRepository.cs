using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IChallanRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessChallanAsync(ProcessChallanRequest request);
    }

}
