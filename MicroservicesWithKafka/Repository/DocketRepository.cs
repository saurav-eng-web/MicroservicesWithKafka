using Dapper;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class DocketRepository : IDocketRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<DocketRepository> _logger;

        public DocketRepository(SQLConnectionService sql,ILogger<DocketRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
         GetDocketAsync(object request)
            {
                using var conn = _sql.GetConnection();
                await conn.OpenAsync();

                using var multi = await conn.QueryMultipleAsync(
                    "SP_CRUD_GetDocketNo",
                    request,
                    commandType: CommandType.StoredProcedure
                );

                var data = new List<dynamic>();
                int statusCode = 200;
                string statusMsg = "Success";

                while (!multi.IsConsumed)
                {
                    var set = (await multi.ReadAsync<dynamic>()).AsList();
                    if (set.Count == 0) continue;

                    var firstRow = (IDictionary<string, object>)set[0];

                    // 🔹 STATUS result set
                    if (firstRow.ContainsKey("StatusCode"))
                    {
                        statusCode = (int)firstRow["StatusCode"];
                        statusMsg = firstRow.ContainsKey("statusMsg")? firstRow["statusMsg"]?.ToString(): firstRow.ContainsKey("Message")? firstRow["Message"]?.ToString(): "No message";
                    }
                    else
                    {
                        // 🔹 DATA result set
                        data.AddRange(set);
                    }
                }

                return (data, statusCode, statusMsg);
            }

    }
}
