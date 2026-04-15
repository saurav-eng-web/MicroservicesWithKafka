using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repositorie
{
    public class PODetailRepository : IPODetailRepository
    {
        private readonly SQLConnectionService _sql;

        public PODetailRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<IEnumerable<dynamic>> ManagePODetailAsync(PODetailRequest request)
        {
            using var con = _sql.GetConnection();

            var param = new DynamicParameters();
            param.Add("@Action", request.Action);
            param.Add("@Id", request.Id);
            param.Add("@POMasterID", request.POMasterID);
            param.Add("@Ticket_Key", request.Ticket_Key);
            param.Add("@ModelID", request.ModelID);
            param.Add("@Partcode", request.Partcode);
            param.Add("@PartDesc", request.PartDesc);
            param.Add("@Price", request.Price);
            param.Add("@qty", request.qty);
            param.Add("@ProcessToBranch", request.ProcessToBranch);
            param.Add("@ERPNO", request.ERPNO);
            param.Add("@DOCNo", request.DOCNo);
            param.Add("@DispatchDate", request.DispatchDate);
            param.Add("@DispatchTime", request.DispatchTime);
            param.Add("@Remark", request.Remark);
            param.Add("@Dispatch", request.Dispatch);
            param.Add("@DispatchQty", request.DispatchQty);
            param.Add("@RequiredQty", request.RequiredQty);
            param.Add("@BalanceQty", request.BalanceQty);
            param.Add("@Backorder", request.Backorder);
            param.Add("@BackorderPO", request.BackorderPO);
            param.Add("@Status", request.Status);
            param.Add("@Recieve", request.Recieve);
            param.Add("@StatusRemark", request.StatusRemark);
            param.Add("@BranchToNWH", request.BranchToNWH);
            param.Add("@SendNWHDate", request.SendNWHDate);
            param.Add("@SendNWH", request.SendNWH);
            param.Add("@SendVendor", request.SendVendor);
            param.Add("@despby", request.despby);
            param.Add("@despid", request.despid);
            param.Add("@Courier", request.Courier);
            param.Add("@CourierAmt", request.CourierAmt);
            param.Add("@RecieveDate", request.RecieveDate);
            param.Add("@PackSlipNo", request.PackSlipNo);
            param.Add("@SFID", request.SFID);
            param.Add("@IsPackingSlipUsed", request.IsPackingSlipUsed);
            param.Add("@NwhSerialNumber", request.NwhSerialNumber);
            param.Add("@InventoryType", request.InventoryType);
            param.Add("@ValueApproval", request.ValueApproval);
            param.Add("@ImageUpload", request.ImageUpload);
            param.Add("@PoCancelReason", request.PoCancelReason);
            param.Add("@PoCancelDate", request.PoCancelDate);

            return await con.QueryAsync<dynamic>("SP_CRUD_PODetail",param,commandType: CommandType.StoredProcedure);
        }
    }
}
