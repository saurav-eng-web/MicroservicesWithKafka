using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IActionMasterService
    {
        Task<string> GetActionsAsync(ActionMaster filterModel);
        Task<int> InsertActionAsync(ActionMaster model);
        Task<string> UpdateActionAsync(ActionMaster model);
        Task<string> DeleteActionAsync(int id);
    }

}
