using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ISqlToMongoSyncService
    {
        Task<int> SyncAsync(DateTime? fromDate, DateTime? toDate, int? regionId);
    }
}
