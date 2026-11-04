using Dapper;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>
    /// Translated from ReportLog.aspx.vb. Legacy ran these SPs through SqlProceedureBackupServer, i.e.
    /// "ConnBackupServer" = a copy of the same SansuiCRM database. The API's reportsConnection points to a
    /// different database (ReposrtServerCRM...), so the main SansuiCRM connection is used here.
    /// </summary>
    public class LogisticReportRepository : ILogisticReportRepository
    {
        // Legacy SqlProceedure used CommandTimeout = 2000000; reports can be slow.
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

        // lnkAvailableDocket -> Report_AvailableDocket (no parameters)
        public Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync() => RunAsync("Report_AvailableDocket");

        // lnkUsedDocket -> Report_UsedDocket (no parameters)
        public Task<IEnumerable<dynamic>> GetUsedDocketReportAsync() => RunAsync("Report_UsedDocket");

        // lnkCompleteDocket -> Report_CompleteDocket (no parameters)
        public Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync() => RunAsync("Report_CompleteDocket");

        // lnkDispatch -> Report_TotalDispatch(@fromdate, @todate)
        public Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate)
            => RunAsync("Report_TotalDispatch", new { fromdate = fromDate, todate = toDate });

        // lnkDispatch1 -> Report_TotalDispatch1(@fromdate, @todate) ("Total Dispatch Header Report")
        public Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate)
            => RunAsync("Report_TotalDispatch1", new { fromdate = fromDate, todate = toDate });
    }
}
