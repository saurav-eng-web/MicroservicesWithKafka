using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IStateMasterService
    {
        Task<StateMaster> CreateStateAsync(StateMaster state);
        Task<bool> UpdateStateAsync(StateMaster state);
        Task<bool> DeleteStateAsync(decimal stateId);
        Task<StateMaster?> GetStateByIdAsync(decimal stateId);
        Task<List<StateMaster>> GetAllStatesAsync();
    }
}
