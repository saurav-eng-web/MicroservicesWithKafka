using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface INwhDefectiveService
    {
        Task<IEnumerable<NwhDefectiveResponse>> ManageNwhDefectiveAsync(NwhDefectiveRequest request);
    }
}
