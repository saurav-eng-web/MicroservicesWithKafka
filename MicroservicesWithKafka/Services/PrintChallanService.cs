using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repositorie;
using MicroservicesWithKafka.Service;

namespace MicroservicesWithKafka.Services
{
    public class PrintChallanService : IPrintChallanService
    {
        private readonly IPrintChallanRepository _repository;

        public PrintChallanService(IPrintChallanRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PrintChallanList>> GetPrintChallanListAsync(PrintChallanList request)
        {
            return await _repository.GetPrintChallanListAsync(request);
        }
    }
}
