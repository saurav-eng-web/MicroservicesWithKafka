using Dapper;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class GetCostRepository : IGetCostRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<GetCostRepository> _logger;

        public GetCostRepository(
            SQLConnectionService sql,
            ILogger<GetCostRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_getCost",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
            {
                var set = (await multi.ReadAsync<dynamic>()).AsList();
                resultSets.Add(set);
            }

            // Last set = Status
            var statusSet = resultSets.LastOrDefault();

            int statusCode = 500;
            string statusMsg = "Unknown error";

            if (statusSet != null && statusSet.Count > 0)
            {
                var row = (IDictionary<string, object>)statusSet[0];

                statusCode = row.ContainsKey("StatusCode")? (int)row["StatusCode"]: 500;

                statusMsg = row.ContainsKey("statusMsg")? row["statusMsg"]?.ToString(): "No message from SP";
            }

            // No data rows in this SP
            var data = new List<dynamic>();

            return (data, statusCode, statusMsg);
        }
    }
}
