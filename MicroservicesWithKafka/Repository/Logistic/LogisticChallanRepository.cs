using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticChallanRepository : ILogisticChallanRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticChallanRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private static string S(IDictionary<string, object?> row, string name) => LogisticRowHelper.Str(row, name);

        private async Task<List<IDictionary<string, object?>>> QueryAsync(string spName, object parameters)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await connection.QueryAsync(spName, parameters, commandType: CommandType.StoredProcedure);
            return LogisticRowHelper.ToRows(rows);
        }

        // Query the SP_CRUD_LogisticChallan branches (the SQL of each legacy page lives in that SP)
        private async Task<List<IDictionary<string, object?>>> QueryChallanAsync(string type, ChallanSearchRequestViewModel request)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticChallan", new
            {
                Type = type,
                request.AscName,
                request.SfId,
                request.Month,
                request.Year
            });
        }

        // ------------------------------------------------------------------
        // Logistic/PrintDefectiveChallan.aspx.vb (menu "Defective Challan")
        // PrintChallanList(@SfId=ascintcode, @Type, @Month, @Year).
        // Legacy grid: Challan No (ponumber, link), SF Name (sfname), Challan Date (date), Doc. No. (status).
        // spType is already mapped to the SP value (Defective, DOA, goodspare, GoodFG, MisFG, MisSpare).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<DefectiveChallanViewModel>> GetPrintChallanListAsync(string sfId, string spType, int month, int year)
        {
            // Legacy passed ddlmonth.SelectedValue / ddlyear.SelectedItem.Text (strings).
            var rows = await QueryAsync("PrintChallanList", new
            {
                SfId = sfId,
                Type = spType,
                Month = month.ToString(),
                Year = year.ToString()
            });
            return rows.Select(ToDefectiveChallan).ToList();
        }

        // ------------------------------------------------------------------
        // Logistic/PrintDOADefectiveChallan.aspx.vb (menu "Defective DOA Challan")
        // ------------------------------------------------------------------
        public async Task<IEnumerable<DefectiveChallanViewModel>> GetDOADefectiveChallansAsync(ChallanSearchRequestViewModel request)
        {
            var rows = await QueryChallanAsync("doa", request);
            return rows.Select(ToDefectiveChallan).ToList();
        }

        private static DefectiveChallanViewModel ToDefectiveChallan(IDictionary<string, object?> r) => new DefectiveChallanViewModel
        {
            Id       = LogisticRowHelper.Int(r, "id"),
            PoNumber = S(r, "ponumber"),
            SfName   = S(r, "sfname"),
            Date     = S(r, "date"),
            Status   = S(r, "status")
        };

        // ------------------------------------------------------------------
        // Logistic/PrintGoodChallan.aspx.vb — exact legacy query (Tbl_GudPOMaster uses Gud_* columns).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodPartChallanViewModel>> GetGoodChallansAsync(ChallanSearchRequestViewModel request)
        {
            var rows = await QueryChallanAsync("good", request);
            return rows.Select(r => new GoodPartChallanViewModel
            {
                GudNo     = S(r, "Gud_No"),
                GudDate   = S(r, "GudDate"),
                GudDocNo  = S(r, "Gud_DocNo"),
                GudRemark = S(r, "Gud_Remark")
            }).ToList();
        }

        // ------------------------------------------------------------------
        // Logistic/PrintGoodChallanMachine.aspx.vb
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodChallanViewModel>> GetGoodMachineChallansAsync(ChallanSearchRequestViewModel request)
        {
            var rows = await QueryChallanAsync("goodmachine", request);
            return rows.Select(r => new GoodChallanViewModel
            {
                Id       = LogisticRowHelper.Int(r, "ID"),
                PONo     = S(r, "POno"),
                Product  = S(r, "product"),
                Model    = S(r, "Model"),
                SerialNo = S(r, "SLNO"),
                TicketNo = S(r, "TN")
            }).ToList();
        }

        // ------------------------------------------------------------------
        // Logistic/PrintMisChallan.aspx.vb
        // ------------------------------------------------------------------
        public async Task<IEnumerable<MisChallanViewModel>> GetMisChallansAsync(ChallanSearchRequestViewModel request)
        {
            var rows = await QueryChallanAsync("mis", request);
            return rows.Select(r => new MisChallanViewModel
            {
                MisNo     = S(r, "Mis_No"),
                MisDate   = S(r, "MisDate"),
                MisDocNo  = S(r, "Mis_DocNo"),
                MisRemark = S(r, "Mis_Remark")
            }).ToList();
        }

        // ------------------------------------------------------------------
        // Logistic/PrintMisChallanFG.aspx.vb — SF = Session("sidfic") in legacy (resolved by the controller).
        // ------------------------------------------------------------------
        public async Task<IEnumerable<GoodChallanViewModel>> GetMisChallanFGAsync(ChallanSearchRequestViewModel request)
        {
            var rows = await QueryChallanAsync("misfg", request);
            return rows.Select(r => new GoodChallanViewModel
            {
                Id       = LogisticRowHelper.Int(r, "ID"),
                PONo     = S(r, "POno"),
                Product  = S(r, "Product"),
                Model    = S(r, "Model"),
                SerialNo = S(r, "SerialNO"),
                TicketNo = S(r, "TicketNo")
            }).ToList();
        }

        // ------------------------------------------------------------------
        // SF dropdowns
        // ------------------------------------------------------------------

        // Legacy PrintDefectiveChallan.aspx.vb: ASClist(@id='', @Role='LOG') -> DataTextField asc_name, DataValueField ascintcode.
        public async Task<IEnumerable<AscOptionViewModel>> GetLogisticAscListAsync()
        {
            var rows = await QueryAsync("ASClist", new { id = "", Role = "LOG" });
            return rows.Select(r => new AscOptionViewModel
            {
                AscIntCode = S(r, "ascintcode"),
                AscName    = S(r, "asc_name")
            }).ToList();
        }

        // Legacy DOA / Good / Good Machine / Mis pages:
        // select ASCINTCODE,ASC_NAME from ASC_MASTER order by ASC_Name
        public async Task<IEnumerable<AscOptionViewModel>> GetAscMasterListAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticChallan", new { Type = "asclist" });
            return rows.Select(r => new AscOptionViewModel
            {
                AscIntCode = S(r, "ASCINTCODE"),
                AscName    = S(r, "ASC_NAME")
            }).ToList();
        }
    }
}
