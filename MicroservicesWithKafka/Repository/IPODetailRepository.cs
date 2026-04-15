using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repositorie
{
    public interface IPODetailRepository
    {
        Task<IEnumerable<dynamic>> ManagePODetailAsync(PODetailRequest request);
    }
}
