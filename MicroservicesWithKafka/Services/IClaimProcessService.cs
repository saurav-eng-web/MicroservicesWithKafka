using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IClaimProcessService
    {
        Task<(IEnumerable<dynamic> Table1,
              IEnumerable<dynamic> Table2,
              int StatusCode,
              string Message)>
        GetClaimDataAsync(ClaimProcessModel model);

        Task<(int StatusCode, string Message)>
        UpdateClaim(ClaimProcessModel model);
    }
}
