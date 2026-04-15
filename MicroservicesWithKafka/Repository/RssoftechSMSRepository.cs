using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class RssoftechSMSRepository : IRssoftechSMSRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public RssoftechSMSRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<bool> InsertSentSMSAsync(RssoftechSMS sms)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("InsertSentSMS", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Source", sms.Source ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Mobile", sms.Mobile ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Message", sms.Message ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ticket_key", sms.TicketKey ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@REF", sms.REF ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Sent", sms.Sent);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }
    }
}
