using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IJobnOtherDetailNewRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetAsync(string ticketNo, string type);
    }
}
