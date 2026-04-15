using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repositorie
{
    public interface IPrintChallanRepository
    {
        Task<IEnumerable<PrintChallanList>> GetPrintChallanListAsync(PrintChallanList request);
    }
}
