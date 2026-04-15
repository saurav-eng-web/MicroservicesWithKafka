using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IJobnOtherDetailNewService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetAsync(string ticketNo, string type);
    }
}
