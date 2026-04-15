using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class Level1OperationRepository : ILevel1OperationRepository
    {
        private readonly SQLConnectionService _sql;

        public Level1OperationRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<Level1OperationResponse> Level1OperationAsync(Level1OperationRequest request)
        {
            using var con = _sql.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TicketNo", request.TicketNo);
            parameters.Add("@UsrName", request.UsrName);
            parameters.Add("@Type", request.Type);
            parameters.Add("@Apptdate", request.Apptdate);
            parameters.Add("@Appttime", request.Appttime);
            parameters.Add("@DOP", request.DOP);
            parameters.Add("@reallot", request.Reallot);
            parameters.Add("@contactPersion", request.ContactPersion);
            parameters.Add("@CustomerRemark", request.CustomerRemark);
            parameters.Add("@UsrId", request.UsrId);
            parameters.Add("@Eng_id", request.Eng_id);
            parameters.Add("@CallTypeID", request.CallTypeID);
            parameters.Add("@CALLTYPE", request.CALLTYPE);

            try
            {
                var result = await con.QueryFirstOrDefaultAsync<Level1OperationResponse>(
                    "SP_CRUD_level1_operation",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result ?? new Level1OperationResponse
                {
                    StatusCode = 500,
                    Message = "Unknown error occurred"
                };
            }
            catch (Exception ex)
            {
                return new Level1OperationResponse
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}
