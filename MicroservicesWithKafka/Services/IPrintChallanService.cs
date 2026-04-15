using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Service
{
    public interface IPrintChallanService
    {
        Task<IEnumerable<PrintChallanList>> GetPrintChallanListAsync(PrintChallanList request);
    }
}
