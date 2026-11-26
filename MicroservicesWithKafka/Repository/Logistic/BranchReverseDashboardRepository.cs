using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class BranchReverseDashboardRepository : IBranchReverseDashboardRepository
    {
        private const string SpName = "SP_CRUD_BranchReverseDashboard";

        private readonly SQLConnectionService _sqlConnectionService;

        public BranchReverseDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<BranchReverseSummaryViewModel> GetSummaryAsync(int branchId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "docketstatus", BranchId = branchId });

            var summary = new BranchReverseSummaryViewModel();
            var row = rows.FirstOrDefault();
            if (row != null)
            {
                summary.PickupAwaited     = LogisticRowHelper.Int(row, "PickupAwaited");
                summary.InTransit         = LogisticRowHelper.Int(row, "InTransit");
                summary.Delivered         = LogisticRowHelper.Int(row, "Delivered");
                summary.ChallanToBeCancel = LogisticRowHelper.Int(row, "ChallanToBeCancel");
            }

            return summary;
        }

        public async Task<IEnumerable<DashboardDetailViewModel>> GetDetailsAsync(int branchId, string bucket)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, SpName,
                new { Type = "details", Bucket = bucket, BranchId = branchId });

            return rows.Select(r => new DashboardDetailViewModel
            {
                SourceType    = LogisticRowHelper.Str(r, "SourceType"),
                ChallanNo     = LogisticRowHelper.Str(r, "ChallanNo"),
                NspName       = LogisticRowHelper.Str(r, "NspName"),
                AscCode       = LogisticRowHelper.Str(r, "AscCode"),
                PickupBoy     = LogisticRowHelper.Str(r, "PickupBoy"),
                PickupMobile  = LogisticRowHelper.Str(r, "PickupMobile"),
                ChallanStatus = LogisticRowHelper.Str(r, "ChallanStatus"),
                DocketStatus  = LogisticRowHelper.Str(r, "DocketStatus")
            }).ToList();
        }
    }
}
