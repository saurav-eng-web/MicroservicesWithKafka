using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class PrintDataRepository : IPrintDataRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public PrintDataRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<object> GetPrintDataAsync(PrintDataRequest request)
        {
            try
            {
                using (var conn = _sqlConnectionService.GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@PONum", request.pono, DbType.String);
                    parameters.Add("@Type", request.Type, DbType.String);
                    parameters.Add("@challantype", request.challantype, DbType.String);

                    string type = request.Type.ToLower();

                    // --------------------------
                    // INVOICE DETAIL
                    // --------------------------
                    if (type == "invoicedetail")
                    {
                        var result = await conn.QueryAsync<PrintDetails>(
                            "SP_CRUD_printdata",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        if (!result.Any())
                        {
                            return new
                            {
                                statusCode = 404,
                                message = "No invoice details found",
                                request
                            };
                        }

                        return result;
                    }

                    // --------------------------
                    // CHALLAN LINE
                    // --------------------------
                    if (type == "challanline")
                    {
                        var result = await conn.QueryAsync<PrintLine>(
                            "SP_CRUD_printdata",
                            parameters,
                            commandType: CommandType.StoredProcedure
                        );

                        if (!result.Any())
                        {
                            return new
                            {
                                statusCode = 404,
                                message = "No challan line data found",
                                request
                            };
                        }

                        return result;
                    }

                    // --------------------------
                    // INVALID TYPE
                    // --------------------------
                    return new
                    {
                        statusCode = 400,
                        message = "Invalid Type. Allowed values: invoicedetail, challanline",
                        request
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    statusCode = 500,
                    message = "Internal server error while fetching print data",
                    error = ex.Message
                };
            }
        }
    }
}
