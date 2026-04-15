using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class StateMasterService : IStateMasterService
    {
        private readonly IStateMasterRepository _stateRepository;

        public StateMasterService(IStateMasterRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<StateMaster> CreateStateAsync(StateMaster state)
        {
            return await _stateRepository.CreateStateAsync(state);
        }

        public async Task<bool> UpdateStateAsync(StateMaster state)
        {
            return await _stateRepository.UpdateStateAsync(state);
        }

        public async Task<bool> DeleteStateAsync(decimal stateId)
        {
            return await _stateRepository.DeleteStateAsync(stateId);
        }

        public async Task<StateMaster?> GetStateByIdAsync(decimal stateId)
        {
            return await _stateRepository.GetStateByIdAsync(stateId);
        }

        public async Task<List<StateMaster>> GetAllStatesAsync()
        {
            return await _stateRepository.GetAllStatesAsync();
        }
    }
}
