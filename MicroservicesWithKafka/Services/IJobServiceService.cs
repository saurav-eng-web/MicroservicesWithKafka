using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IJobServiceService
    {
        Task<JobService> CreateJobAsync(JobService job);
    }
}
