using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ClaimProcessRepository : IClaimProcessRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<ClaimProcessRepository> _logger;

        public ClaimProcessRepository(SQLConnectionService sql, ILogger<ClaimProcessRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Table1,IEnumerable<dynamic> Table2,int StatusCode,string Message)>
 ExecuteClaimProcessAsync(ClaimProcessModel model)
        {
            try
            {
                using var connection = _sql.GetConnection();
                await connection.OpenAsync();

                using var multi = await connection.QueryMultipleAsync(
                    "SP_CRUD_claimprocess",
                    new
                    {
                        userid = model.UserId,
                        type = model.Type,
                        action = model.Action,
                        recid = model.RecId,
                        remarks = model.Remarks,
                        statusreason = model.StatusReason,
                        Dop = model.DopInput,
                        Modelno = model.ModelNoInput,
                        serial_no = model.SerialNoInput,
                        nsp = model.Nsp,
                        ct = model.Ct,
                        sst = model.Sst,
                        TK1 = model.Tk1,
                        fromdate = model.FromDate,
                        todate = model.ToDate
                    },
                    commandType: CommandType.StoredProcedure
                );

                // Read actual result sets dynamically (NOT mapped to model)
                var table1 = (await multi.ReadAsync<dynamic>()).AsList();
                var table2 = (await multi.ReadAsync<dynamic>()).AsList();

                int statusCode = 500;
                string message = "No response from stored procedure";

                // Extract status from second table
                if (table2.Count > 0)
                {
                    dynamic statusRow = table2[0];
                    statusCode = statusRow.StatusCode;
                    message = statusRow.Message;
                }

                return (table1, table2, statusCode, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExecuteClaimProcessAsync");
                throw new Exception($"Database error: {ex.Message}");
            }
        }


        public async Task<(int StatusCode, string Message)> UpdateClaimAsync(ClaimProcessModel model)
        {
            try
            {
                using var conn = _sql.GetConnection();
                await conn.OpenAsync();

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_CRUD_claimprocess",
                    new
                    {
                        userid = model.UserId,
                        type = model.Type,
                        action = model.Action,
                        recid = model.RecId,
                        remarks = model.Remarks,
                        statusreason = model.StatusReason,
                        dop = model.DopInput,
                        modelno = model.ModelNoInput,
                        serial_no = model.SerialNoInput,
                        nsp = model.Nsp,
                        ct = model.Ct,
                        sst = model.Sst,
                        tk1 = model.Tk1,
                        fromdate = model.FromDate,
                        todate = model.ToDate
                    },
                    commandType: CommandType.StoredProcedure
                );

                return (
                    result?.StatusCode ?? 500,
                    result?.Message ?? "No response from stored procedure"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateClaimAsync failed");
                return (500, ex.Message);
            }
        }
    }
}
