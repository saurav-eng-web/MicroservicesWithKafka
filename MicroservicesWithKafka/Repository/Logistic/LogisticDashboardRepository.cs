using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
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
            var row = LogisticRowHelper.ToRows(counts).FirstOrDefault();
            if (row != null)
            {
                string Count(string column)
                {
                    var text = LogisticRowHelper.Str(row, column);
                    return text.Length > 0 ? text : "0";
                }

                summary.DefectivePart   = Count("Tbl_DefPOMaster");
                summary.DefectiveDOA    = Count("tbl_DOADefectiveNWH");
                summary.GoodPart        = Count("Tbl_GudPOMaster");
                summary.GoodMachine     = Count("tbl_SfFGStock");
                summary.MismatchPart    = Count("Tbl_MisPOMaster");
                summary.MismatchMachine = Count("tbl_SfFGStock");
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
