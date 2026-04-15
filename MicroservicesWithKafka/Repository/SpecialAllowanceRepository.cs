using Dapper;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class SpecialAllowanceRepository : ISpecialAllowanceRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<SpecialAllowanceRepository> _logger;

        public SpecialAllowanceRepository(
            SQLConnectionService sql,
            ILogger<SpecialAllowanceRepository> logger)
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
                "SP_CRUD_SpecialAllowance",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            // 🔑 Read ALL result sets safely
            while (!multi.IsConsumed)
            {
                var set = (await multi.ReadAsync<dynamic>()).AsList();
                resultSets.Add(set);
            }

            // 🔑 Last result set = Status
            var statusSet = resultSets.LastOrDefault();

            int statusCode = 500;
            string statusMsg = "Unknown error";

            if (statusSet != null && statusSet.Count > 0)
            {
                var row = (IDictionary<string, object>)statusSet[0];
                statusCode = row.ContainsKey("StatusCode")? (int)row["StatusCode"]: 500;
                statusMsg = row.ContainsKey("StatusMsg")? row["StatusMsg"]?.ToString(): "No status message";
            }

            // 🔑 All data EXCEPT status
            var data = resultSets.Count > 1? resultSets.Take(resultSets.Count - 1).SelectMany(x => x): new List<dynamic>();
            return (data, statusCode, statusMsg);
        }
    }
}
