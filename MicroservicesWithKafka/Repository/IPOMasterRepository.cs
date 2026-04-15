using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repositorie
{
    public interface IPOMasterRepository
    {
        Task<IEnumerable<dynamic>> ManagePOMasterAsync(POMasterRequest request);
    }

}
