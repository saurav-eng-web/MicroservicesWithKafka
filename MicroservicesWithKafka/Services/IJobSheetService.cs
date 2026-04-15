using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IJobSheetService
    {
        Task<int> ProcessJobSheetAsync(JobSheetRequest request);
    }

}
