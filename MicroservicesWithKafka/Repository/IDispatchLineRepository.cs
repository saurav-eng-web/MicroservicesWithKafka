using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IDispatchLineRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMessage)>
            ExecuteAsync(DispatchLineRequest request);
    }
}
