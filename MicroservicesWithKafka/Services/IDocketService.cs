using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IDocketService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetDocketAsync(object request);
    }
}
