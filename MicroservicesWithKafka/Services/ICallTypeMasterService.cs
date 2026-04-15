using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICallTypeMasterService
    {
        Task<IEnumerable<CallTypeMaster>> GetAllAsync();
        Task<CallTypeMaster?> GetByIdAsync(long id);
        Task<long> AddAsync(CallTypeMaster callType);
        Task<long> UpdateAsync(CallTypeMaster callType);
        Task<long> DeleteAsync(long id);
    }
}
