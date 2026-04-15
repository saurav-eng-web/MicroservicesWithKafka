using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface ISFReceivePOService
    {
        Task<IEnumerable<dynamic>> ManageSFReceivePOAsync(SFReceivePORequest request);
    }
}
