using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface ISFReceivePORepository
    {
        Task<IEnumerable<dynamic>> ManageSFReceivePOAsync(SFReceivePORequest request);
    }
}
