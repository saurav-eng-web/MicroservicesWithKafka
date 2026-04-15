using MicroservicesWithKafka.Models.Requests;
using MicroservicesWithKafka.Models.Responses;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class DropdownMasterService : IDropdownMasterService
    {
        private readonly IDropdownMasterRepository _repo;

        public DropdownMasterService(IDropdownMasterRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<DropdownResponse>> GetDropdownAsync(DropdownRequest request)
        {
            return await _repo.GetDropdownAsync(request);
        }
    }
}
