using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class JobServiceService : IJobServiceService
    {
        private readonly IJobServiceRepository _repository;

        public JobServiceService(IJobServiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobService> CreateJobAsync(JobService job)
        {
            return await _repository.CreateJobAsync(job);
        }
    }

}
