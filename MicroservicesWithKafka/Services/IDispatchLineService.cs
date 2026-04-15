using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IDispatchLineService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string Message)>
            ProcessAsync(DispatchLineRequest request);
    }
}
