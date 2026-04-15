using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ISpecialAllowanceService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessAsync(object request);
    }
}
