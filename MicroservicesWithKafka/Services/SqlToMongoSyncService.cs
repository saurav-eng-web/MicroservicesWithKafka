using System;
using System.Threading.Tasks;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class SqlToMongoSyncService : ISqlToMongoSyncService
    {
        private readonly ISqlToMongoSyncRepository _repository;

        public SqlToMongoSyncService(ISqlToMongoSyncRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> SyncAsync(DateTime? fromDate, DateTime? toDate, int? regionId)
        {
            return await _repository.SyncSqlToMongoAsync(fromDate, toDate, regionId);
        }
    }
}
