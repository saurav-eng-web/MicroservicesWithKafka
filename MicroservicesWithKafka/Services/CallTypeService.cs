using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CallTypeService : ICallTypeService
    {
        private readonly ICallTypeRepository _repository;

        public CallTypeService(ICallTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CallTypeMaster>> GetCallTypesBySubcategoryAsync(string subCategory)
        {
            return await _repository.GetCallTypesBySubcategoryAsync(subCategory);
        }
    }
}
