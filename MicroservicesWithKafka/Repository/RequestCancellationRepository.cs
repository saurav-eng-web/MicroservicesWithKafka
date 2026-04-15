using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class RequestCancellationRepository : IRequestCancellationRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public RequestCancellationRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<RequestCancellation> CreateAsync(RequestCancellation model)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "INSERT");
            parameters.Add("@TransactionType", model.TransactionType);
            parameters.Add("@TransactionID", model.TransactionID);
            parameters.Add("@TransactionNo", model.TransactionNo);
            parameters.Add("@RequestedBy", model.RequestedBy);
            parameters.Add("@Reason", model.Reason);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@ApprovalStatus", model.ApprovalStatus);
            parameters.Add("@CreatedBy", model.CreatedBy);
            parameters.Add("@IPAddress", model.IPAddress);

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
              "SP_CRUD_Tbl_CancellationRequest",
              parameters,
              commandType: CommandType.StoredProcedure
            );

            model.CancellationRequestID = (int?)result?.NewCancellationRequestID;
            model.StatusMsg = result?.StatusMsg;
            model.StatusCode = result?.StatusCode; // ⭐ Assign the StatusCode

            return model;
        }


        public async Task<RequestCancellation> UpdateAsync(RequestCancellation model)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "UPDATE");
            parameters.Add("@CancellationRequestID", model.CancellationRequestID);
            parameters.Add("@TransactionType", model.TransactionType);
            parameters.Add("@TransactionID", model.TransactionID);
            parameters.Add("@TransactionNo", model.TransactionNo);
            parameters.Add("@RequestedBy", model.RequestedBy);
            parameters.Add("@Reason", model.Reason);
            parameters.Add("@Remarks", model.Remarks);
            parameters.Add("@ApprovalStatus", model.ApprovalStatus);
            parameters.Add("@ApprovedBy", model.ApprovedBy);
            parameters.Add("@ApprovedDate", model.ApprovedDate);
            parameters.Add("@ApprovalRemarks", model.ApprovalRemarks);
            parameters.Add("@ModifiedBy", model.ModifiedBy);
            parameters.Add("@IPAddress", model.IPAddress);

            var result = await connection.ExecuteScalarAsync<string>(
              "SP_CRUD_Tbl_CancellationRequest",
              parameters,
              commandType: CommandType.StoredProcedure
            );

            model.StatusMsg = result; // e.g. "Record updated successfully"
            model.StatusCode = 201;
            return model;
        }


        public async Task<RequestCancellation> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "DELETE");
            parameters.Add("@CancellationRequestID", id);

            var msg = await connection.ExecuteScalarAsync<string>(
              "SP_CRUD_Tbl_CancellationRequest",
              parameters,
              commandType: CommandType.StoredProcedure
            );

            return new RequestCancellation
            {
                CancellationRequestID = id,
                StatusMsg = msg
            };
        }


        public async Task<IEnumerable<RequestCancellation>> GetAllAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();

            var result = await connection.QueryAsync<RequestCancellation>(
              "SP_CRUD_Tbl_CancellationRequest",
              new { Action = "SELECT" },
              commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<RequestCancellation?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var result = await connection.QueryAsync<RequestCancellation>(
              "SP_CRUD_Tbl_CancellationRequest",
              new { Action = "SELECT", CancellationRequestID = id },
              commandType: CommandType.StoredProcedure
            );

            return result.FirstOrDefault();
        }
    }
}