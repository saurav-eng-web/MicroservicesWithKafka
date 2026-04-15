using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class JobSheetService : IJobSheetService
    {
        private readonly IJobSheetRepository _repository;

        public JobSheetService(IJobSheetRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> ProcessJobSheetAsync(JobSheetRequest request)
        {
            return await _repository.ExecuteJobSheetAsync(request);
        }
    }

}
