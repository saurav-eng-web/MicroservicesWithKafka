using Dapper;
using System.Data;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;

namespace MicroservicesWithKafka.Repositorie
{
    public class PrintChallanRepository : IPrintChallanRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public PrintChallanRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<PrintChallanList>> GetPrintChallanListAsync(PrintChallanList request)
        {
            try
            {
                using var connection = _sqlConnectionService.GetConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@SfId", request.SfId);
                parameters.Add("@Type", request.Type);
                parameters.Add("@Month", request.Month);
                parameters.Add("@Year", request.Year);

                // ✅ Execute stored procedure
                var result = (await connection.QueryAsync<PrintChallanList>(
                    "SP_CRUD_PrintChallanList",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                // ✅ If no data returned, send message accordingly
                if (result == null || !result.Any())
                {
                    return new List<PrintChallanList>
                    {
                        new PrintChallanList
                        {
                            Message = "No records found",
                            Status = "404"
                        }
                    };
                }

                // ✅ Add a success message to all rows
                foreach (var item in result)
                {
                    item.Message ??= "Data fetched successfully";
                    item.Status ??= "200";
                }

                return result;
            }
            catch (Exception ex)
            {
                // ✅ Return user-friendly error response
                return new List<PrintChallanList>
                {
                    new PrintChallanList
                    {
                        Message = $"Error: {ex.Message}",
                        Status = "500"
                    }
                };
            }
        }
    }
}
