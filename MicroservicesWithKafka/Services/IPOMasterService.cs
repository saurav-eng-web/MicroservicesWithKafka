using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IPOMasterService
    {
        Task<IEnumerable<dynamic>> ManagePOMasterAsync(POMasterRequest request);
    }

}
