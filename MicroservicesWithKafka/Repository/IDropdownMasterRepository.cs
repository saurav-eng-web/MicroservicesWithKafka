using MicroservicesWithKafka.Models.Requests;
using MicroservicesWithKafka.Models.Responses;

namespace MicroservicesWithKafka.Repository
{
    public interface IDropdownMasterRepository
    {
        Task<IEnumerable<DropdownResponse>> GetDropdownAsync(DropdownRequest request);
    }
}
