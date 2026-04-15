using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IMenuMasterRepository
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string Message)>
            ExecuteAsync(MenuMasterRequest model);
    }
}
