using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IMenuMasterService
    {
        Task<(IEnumerable<dynamic> Data, int StatusCode, string Message)>
            ProcessAsync(MenuMasterRequest model);
    }
}
