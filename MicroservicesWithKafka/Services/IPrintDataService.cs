using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IPrintDataService
    {
        Task<object> FetchAsync(PrintDataRequest request);
    }

}
