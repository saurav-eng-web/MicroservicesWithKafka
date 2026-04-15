using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IClaimProcessRepository
    {
        Task<(IEnumerable<dynamic> Table1,
      IEnumerable<dynamic> Table2,
      int StatusCode, string Message)>
ExecuteClaimProcessAsync(ClaimProcessModel model);


        Task<(int StatusCode, string Message)> UpdateClaimAsync(ClaimProcessModel model);
    }
}
