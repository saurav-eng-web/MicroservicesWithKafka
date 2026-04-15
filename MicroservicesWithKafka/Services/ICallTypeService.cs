using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ICallTypeService
    {
        Task<List<CallTypeMaster>> GetCallTypesBySubcategoryAsync(string subCategory);
    }
}
