using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticUploadRepository
    {
        Task<int> BulkInsertAsync(List<LogisticUploadModel> rows);
    }
}
