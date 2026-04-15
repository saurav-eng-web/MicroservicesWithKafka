using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ComplaintHistorySqlService
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ComplaintHistorySqlService(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<long> InsertAsync(ComplaintHistory model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "INSERT");
            command.Parameters.AddWithValue("@Ticket_No", model.TicketNo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_status", model.ComplaintStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_subStatus", model.ComplaintSubStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", DateTime.Now);
            command.Parameters.AddWithValue("@EnteredBy", model.EnteredBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredByID", model.EnteredByID ?? (object)DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }
    }
}
