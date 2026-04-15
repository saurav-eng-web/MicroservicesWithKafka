using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IAscPendingChallanService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMessage)>
            ProcessAsync(int id, string type);
    }
}
