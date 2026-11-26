using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class LogisticDashboardRepository : ILogisticDashboardRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            var summary = new DashboardSummaryViewModel();

            using var connection = _sqlConnectionService.GetConnection();

            var counts = await connection.QueryAsync("sp_LogisticDashBoard", commandType: CommandType.StoredProcedure);
            if (counts.FirstOrDefault() is IDictionary<string, object?> rawRow)
            {
                var pairs = rawRow.ToList();

                string Count(string column, int occurrence = 0)
                {
                    var matches = pairs.Where(p => string.Equals(p.Key, column, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (matches.Count == 0) return "0";
                    var value = (occurrence < matches.Count ? matches[occurrence] : matches[0]).Value;
                    var text = value == null || value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
                    return text.Length > 0 ? text : "0";
                }

                summary.DefectivePart   = Count("Tbl_DefPOMaster");
                summary.DefectiveDOA    = Count("tbl_DOADefectiveNWH");
                summary.GoodPart        = Count("Tbl_GudPOMaster");
                summary.GoodMachine     = Count("tbl_SfFGStock", 0);
                summary.MismatchPart    = Count("Tbl_MisPOMaster");
                summary.MismatchMachine = Count("tbl_SfFGStock", 1);
            }

            var availability = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDashboard",
                new { Type = "courieravailability" });
            summary.CourierAvailability.AddRange(availability.Select(r => new CourierAvailabilityViewModel
            {
                CourierName = LogisticRowHelper.Str(r, "CourierName"),
                Avi         = LogisticRowHelper.Int(r, "Avi")
            }));

            var docketStatus = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDashboard",
                new { Type = "docketstatus" });
            var statusRow = docketStatus.FirstOrDefault();
            if (statusRow != null)
            {
                summary.PickupAwaited = LogisticRowHelper.Int(statusRow, "PickupAwaited");
                summary.InTransit     = LogisticRowHelper.Int(statusRow, "InTransit");
                summary.Delivered     = LogisticRowHelper.Int(statusRow, "Delivered");
                summary.ChallanToBeCancel = LogisticRowHelper.Int(statusRow, "ChallanToBeCancel");
            }

            return summary;
        }

        public async Task<IEnumerable<DashboardDetailViewModel>> GetDashboardDetailsAsync(string bucket)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDashboard",
                new { Type = "details", Bucket = bucket });

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
