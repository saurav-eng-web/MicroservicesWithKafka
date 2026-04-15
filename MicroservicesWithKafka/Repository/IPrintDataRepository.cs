using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IPrintDataRepository
    {
        Task<object> GetPrintDataAsync(PrintDataRequest request);
    }

}
