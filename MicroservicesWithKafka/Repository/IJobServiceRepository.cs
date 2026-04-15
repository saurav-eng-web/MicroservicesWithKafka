using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IJobServiceRepository
    {
        Task<JobService> CreateJobAsync(JobService job);
    }
}
