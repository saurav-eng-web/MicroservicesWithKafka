using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class PrintDataService : IPrintDataService
    {
        private readonly IPrintDataRepository _repo;

        public PrintDataService(IPrintDataRepository repo)
        {
            _repo = repo;
        }

        public async Task<object> FetchAsync(PrintDataRequest request)
        {
            return await _repo.GetPrintDataAsync(request);
        }
    }
}
