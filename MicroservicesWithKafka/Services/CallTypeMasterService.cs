using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CallTypeMasterService : ICallTypeMasterService
    {
        private readonly ICallTypeMasterRepository _repository;

        public CallTypeMasterService(ICallTypeMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CallTypeMaster>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<CallTypeMaster?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<long> AddAsync(CallTypeMaster callType)
        {
            return await _repository.AddAsync(callType);
        }

        public async Task<long> UpdateAsync(CallTypeMaster callType)
        {
            return await _repository.UpdateAsync(callType);
        }

        public async Task<long> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
