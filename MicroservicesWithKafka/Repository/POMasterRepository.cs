using Dapper;
using System.Data;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;

namespace MicroservicesWithKafka.Repositorie
{
    public class POMasterRepository : IPOMasterRepository
    {
        private readonly SQLConnectionService _sql;

        public POMasterRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<IEnumerable<dynamic>> ManagePOMasterAsync(POMasterRequest request)
        {
            using var con = _sql.GetConnection();

            var param = new DynamicParameters();
            param.Add("@Action", request.Action);
            param.Add("@Id", request.Id);
            param.Add("@PONumber", request.PONumber);
            param.Add("@Ticket_No", request.Ticket_No);
            param.Add("@Ticket_Key", request.Ticket_Key);
            param.Add("@SFID", request.SFID);
            param.Add("@Status", request.Status);
            param.Add("@StatusRemark", request.StatusRemark);
            param.Add("@BranchToNWH", request.BranchToNWH);
            param.Add("@SendNWHDate", request.SendNWHDate);
            param.Add("@SendNWH", request.SendNWH);
            param.Add("@SendVendor", request.SendVendor);
            param.Add("@InvoiceNo", request.InvoiceNo);
            param.Add("@Courier", request.Courier);
            param.Add("@TOInvSF", request.TOInvSF);
            param.Add("@TOInvSFid", request.TOInvSFid);
            param.Add("@CourierAmt", request.CourierAmt);
            param.Add("@Nwhcatid", request.Nwhcatid);
            param.Add("@InventoryType", request.InventoryType);
            param.Add("@PoType", request.PoType);
            param.Add("@PoCancelDate", request.PoCancelDate);
            param.Add("@PoCancelReason", request.PoCancelReason);

            return await con.QueryAsync<dynamic>(
                "SP_CRUD_POMaster",
                param,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
