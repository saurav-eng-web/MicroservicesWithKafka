using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ICallTypeRepository
    {
        Task<List<CallTypeMaster>> GetCallTypesBySubcategoryAsync(string subCategory);
    }
}
