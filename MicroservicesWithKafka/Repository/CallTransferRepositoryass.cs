using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
namespace MicroservicesWithKafka.Repository
{
   
    public class CallTransferRepository : ICallTransferRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CallTransferRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<CallTransferResponse> TransferCallAsync(CallTransferRequest request)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("Transfercall", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@TicketNo", request.TicketNo);
            command.Parameters.AddWithValue("@ToSfId", request.ToSfId);
            command.Parameters.AddWithValue("@TransferBy", request.TransferBy);
            command.Parameters.AddWithValue("@TransferTo", request.TransferTo);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CallTransferResponse
                {
                    Status = reader["status"].ToString()
                };
            }

            return new CallTransferResponse
            {
                Status = "No response from procedure"
            };
        }
    }
}
