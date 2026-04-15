using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using Dapper; // ✨ Add Dapper

namespace MicroservicesWithKafka.Repository
{
    // The IComplaintCustDetailRepository and SQLConnectionService remain the same.

    public class ComplaintCustDetailRepository : IComplaintCustDetailRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ComplaintCustDetailRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ComplaintCustDetail?> GetByTicketNoAsync(string ticketNo)
        {
            // Dapper handles the heavy lifting of connection management (mostly)
            // and mapping the results to your ComplaintCustDetail model.
            using (var connection = _sqlConnectionService.GetConnection())
            {
                // Note: We don't need to manually call connection.OpenAsync() 
                // as Dapper typically handles opening the connection if it's closed 
                // when executing a query.

                var parameters = new DynamicParameters();
                // Using DynamicParameters is safer and cleaner than AddWithValue
                parameters.Add("@TicketNo", ticketNo ?? (object)DBNull.Value, DbType.String);
                parameters.Add("@type", "CompCust", DbType.String);

                // QueryFirstOrDefaultAsync executes the stored procedure and maps the first 
                // result row to your ComplaintCustDetail model, returning null if no rows are found.
                var result = await connection.QueryFirstOrDefaultAsync<ComplaintCustDetail>(
                    "SP_CRUD_ComplaintCustDetail",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // Convert string ("Yes", "No", "---Select---") → int?
                result.Wallmount_Available = ConvertStringToNormalizedString(result.Wallmount_Available);

                // Convert bool? → int?
               

                return result;
            }
        }

        // Converts possible string values
        private string? ConvertStringToNormalizedString(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var v = value.Trim().ToLower();

            return v switch
            {
                "yes" => "1",
                "1" => "1",
                "true" => "1",
                "no" => "0",
                "0" => "0",
                "false" => "0",
                "---select---" => null,
                _ => value
            };
        }

        private int? ConvertBoolToInt(bool? value)
        {
            if (value == null) return null;
            return value == true ? 1 : 0;
        }
    }
}