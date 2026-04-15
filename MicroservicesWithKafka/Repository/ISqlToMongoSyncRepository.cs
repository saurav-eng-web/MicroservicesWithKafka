using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ISqlToMongoSyncRepository
    {
        /// <summary>
        /// Sync CallRegister data from SQL Server to MongoDB using stored procedure.
        /// </summary>
        /// <param name="fromDate">Optional start date.</param>
        /// <param name="toDate">Optional end date.</param>
        /// <param name="regionId">Optional region filter.</param>
        /// <returns>Number of records synced.</returns>
        Task<int> SyncSqlToMongoAsync(DateTime? fromDate, DateTime? toDate, int? regionId);
    }
}
