using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticDashboardRepository : ILogisticDashboardRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticDashboardRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // Translated from Dashboard.aspx.vb Page_Load – sp_LogisticDashBoard (challan counts) + SP_CRUD_LogisticDashboard (courier availability)
        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            var summary = new DashboardSummaryViewModel();

            using var connection = _sqlConnectionService.GetConnection();

            // Step 1: challan counts. Column names taken exactly from Dashboard.aspx.vb ds2.Tables(0).Rows(0).Item(...)
            var counts = await connection.QueryAsync("sp_LogisticDashBoard", commandType: CommandType.StoredProcedure);
            if (counts.FirstOrDefault() is IDictionary<string, object?> rawRow)
            {
                // sp_LogisticDashBoard returns the column tbl_SfFGStock TWICE: the 1st is Good Machine (MiscBin = 0) and the
                // 2nd is Mismatch Machine (MiscBin = 1). The columns are read by position within the same name.
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

            // Step 2: courier availability counts
            var availability = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDashboard",
                new { Type = "courieravailability" });
            summary.CourierAvailability.AddRange(availability.Select(r => new CourierAvailabilityViewModel
            {
                CourierName = LogisticRowHelper.Str(r, "CourierName"),
                Avi         = LogisticRowHelper.Int(r, "Avi")
            }));

            return summary;
        }
    }
}
