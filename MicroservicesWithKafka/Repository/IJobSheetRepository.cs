using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IJobSheetRepository
    {
        Task<int> ExecuteJobSheetAsync(JobSheetRequest request);
    }
}
