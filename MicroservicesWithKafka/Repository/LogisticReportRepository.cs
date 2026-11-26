using Dapper;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class LogisticReportRepository : ILogisticReportRepository
    {
        private const int CommandTimeoutSeconds = 600;

        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticReportRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private async Task<IEnumerable<dynamic>> RunAsync(string spName, object? parameters = null)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await connection.QueryAsync(spName, parameters,
                commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeoutSeconds);
        }

        public Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync() => RunAsync("Report_AvailableDocket");

        public Task<IEnumerable<dynamic>> GetUsedDocketReportAsync() => RunAsync("Report_UsedDocket");

        public Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync() => RunAsync("Report_CompleteDocket");

        public Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate)
            => RunAsync("Report_TotalDispatch", new { fromdate = fromDate, todate = toDate });

        public Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate)
            => RunAsync("Report_TotalDispatch1", new { fromdate = fromDate, todate = toDate });
    }
}
