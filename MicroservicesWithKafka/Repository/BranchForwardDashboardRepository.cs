using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class BranchForwardDashboardRepository : IBranchForwardDashboardRepository
    {
        private const string SpName = "SP_CRUD_BranchForwardDashboard";

        private readonly SQLConnectionService _sqlConnectionService;

        public BranchForwardDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ForwardDashboardSummaryViewModel> GetSummaryAsync(DashboardScope scope)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "docketstatus", scope.BranchId, scope.SfId });

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

        public async Task<IEnumerable<Dictionary<string, string>>> GetDetailsAsync(DashboardScope scope, string bucket)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "details", Bucket = bucket, scope.BranchId, scope.SfId });

            return LogisticRowHelper.ToTable(rows);
        }

        public async Task<IEnumerable<Dictionary<string, string>>> GetChallanAsync(DashboardScope scope, string challanNo)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "challan", ChallanNo = challanNo, scope.BranchId, scope.SfId });

            return LogisticRowHelper.ToTable(rows);
        }
    }
}
