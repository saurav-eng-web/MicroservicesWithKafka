using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IAscPendingChallanRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMessage)>
            ExecuteAsync(int id, string type);
    }
}
