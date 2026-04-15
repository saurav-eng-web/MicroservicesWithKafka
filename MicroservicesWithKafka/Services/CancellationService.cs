using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CancellationService : ICancellationService
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CancellationService(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<bool> HandleCancellationRequestAsync(
            string TICKET_NO,
            string remark,
            string type,
            string action,
            string remarkS,
            int uid)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_CancelationRequest", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TicketNo", TICKET_NO);
                    command.Parameters.AddWithValue("@Remark", remark ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Type", type ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Action", action ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RemarkS", remarkS ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Uid", uid);

                    var rows = await command.ExecuteNonQueryAsync();
                    return rows > 0; // ✅ return true if SP executed successfully
                }
            }
        }
    }
}
