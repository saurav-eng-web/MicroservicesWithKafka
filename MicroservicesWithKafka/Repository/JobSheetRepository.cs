using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class JobSheetRepository : IJobSheetRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public JobSheetRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<int> ExecuteJobSheetAsync(JobSheetRequest request)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_JobSheet", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add parameters exactly as in your SP
            command.Parameters.AddWithValue("@TicketNo", request.TICKET_NO ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Apptdate", request.Apptdate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Appttime", request.Appttime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DOP", request.DOP ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Reallot", request.Reallot.HasValue ? request.Reallot.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@Type", request.Type ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Type1", request.Type1 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@contactPersion", request.ContactPersion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CustomerRemark", request.CustomerRemark ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@UsrId", request.UsrId);
            command.Parameters.AddWithValue("@Eng_id", request.Eng_Id.HasValue ? request.Eng_Id.Value : (object)DBNull.Value);
            command.Parameters.AddWithValue("@CallTypeID",request.CallTypeID);
            command.Parameters.AddWithValue("@CALLTYPE", request.CallType ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Action", request.Action ?? (object)DBNull.Value);

            // If your SP returns rows affected only
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected;
        }
    }
}
