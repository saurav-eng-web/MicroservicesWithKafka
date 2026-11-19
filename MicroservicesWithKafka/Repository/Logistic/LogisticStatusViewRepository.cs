using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticStatusViewRepository
    {
        // Translated from StatusView.aspx.vb Page_Load — each type maps to a specific table query
        Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type);
    }

    public class LogisticStatusViewRepository : ILogisticStatusViewRepository
    {
        private static readonly HashSet<string> ValidTypes = new HashSet<string>
        {
            "DefectivePart", "DefectiveDOA", "GoodPart", "GoodMachine", "MismatchPart", "MismatchMachine"
        };

        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticStatusViewRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // StatusView.aspx.vb Page_Load — the 6 query branches by type now live in SP_CRUD_LogisticStatusView
        public async Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type)
        {
            if (!ValidTypes.Contains(type))
                return new List<StatusViewViewModel>();

            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticStatusView", new { Type = type });

            return rows.Select(r => new StatusViewViewModel
            {
                ChallanNo     = LogisticRowHelper.Str(r, "ChallanNo"),
                TypeOfChallan = LogisticRowHelper.Str(r, "TypeOfChallan"),
                PickupBoy     = LogisticRowHelper.Str(r, "PickupBoy"),
                PickupMobile  = LogisticRowHelper.Str(r, "PickupMobile"),
                DocketStatus  = LogisticRowHelper.Str(r, "DocketStatus"),
                ChallanStatus = LogisticRowHelper.Str(r, "ChallanStatus")
            }).ToList();
        }
    }
}
