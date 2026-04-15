using MicroservicesWithKafka.Models.Requests;
using MicroservicesWithKafka.Models.Responses;

namespace MicroservicesWithKafka.Services
{
    public interface IDropdownMasterService
    {
        Task<IEnumerable<DropdownResponse>> GetDropdownAsync(DropdownRequest request);
    }
}
