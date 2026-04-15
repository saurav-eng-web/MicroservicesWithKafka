using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ActionMasterService : IActionMasterService
    {
        private readonly IActionMasterRepository _repo;

        public ActionMasterService(IActionMasterRepository repo)
        {
            _repo = repo;
        }

        public Task<string> GetActionsAsync(ActionMaster filterModel)
            => _repo.GetActionsAsync(filterModel);

        public Task<int> InsertActionAsync(ActionMaster model)
            => _repo.InsertActionAsync(model);

        public Task<string> UpdateActionAsync(ActionMaster model)
            => _repo.UpdateActionAsync(model);

        public Task<string> DeleteActionAsync(int id)
            => _repo.DeleteActionAsync(id);
    }

}

