using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class LogisticForwardDashboardRepository : ILogisticForwardDashboardRepository
    {
        private const string SpName = "SP_CRUD_LogisticForwardDashboard";

        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticForwardDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ForwardDashboardSummaryViewModel> GetSummaryAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName, new { Type = "docketstatus" });

            // No row means the procedure on this database does not know the request: report it, do not show zeros
            var row = rows.FirstOrDefault()
                ?? throw new System.InvalidOperationException($"{SpName} returned no summary row.");

            return new ForwardDashboardSummaryViewModel
            {
                PickupAwaited  = LogisticRowHelper.Int(row, "PickupAwaited"),
                InTransit      = LogisticRowHelper.Int(row, "InTransit"),
                Delivered      = LogisticRowHelper.Int(row, "Delivered"),
                ReceiptPending = LogisticRowHelper.Int(row, "ReceiptPending")
            };
        }

        public async Task<IEnumerable<Dictionary<string, string>>> GetDetailsAsync(string bucket)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "details", Bucket = bucket });

            return LogisticRowHelper.ToTable(rows);
        }

        public async Task<IEnumerable<Dictionary<string, string>>> GetChallanAsync(string challanNo)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "challan", ChallanNo = challanNo });

            return LogisticRowHelper.ToTable(rows);
        }
    }
}
