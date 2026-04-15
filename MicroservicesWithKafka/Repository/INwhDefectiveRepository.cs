using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repositories
{
    public interface INwhDefectiveRepository
    {
        Task<IEnumerable<NwhDefectiveResponse>> ManageNwhDefectiveAsync(NwhDefectiveRequest request);
    }
}
