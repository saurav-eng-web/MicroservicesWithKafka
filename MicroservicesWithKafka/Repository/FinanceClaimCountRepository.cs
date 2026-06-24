using Dapper;
using MicroservicesWithKafka.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{

    public class FinanceClaimCountRepository : IFinanceClaimCountRepository
    {
        private readonly SQLConnectionService _sql;

        public FinanceClaimCountRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(List<List<dynamic>>, int, string)> ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_GetFinanceClaimCounts",
                request,
                commandType: CommandType.StoredProcedure
            );

            var sets = new List<List<dynamic>>();
            while (!multi.IsConsumed)
            {
                sets.Add((await multi.ReadAsync<dynamic>()).AsList());
            }

            int code = 500;
            string msg = "Stored procedure did not return status";

            var statusSet = sets.LastOrDefault();
            if (statusSet != null && statusSet.Count > 0 && statusSet[0] is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc)) code = Convert.ToInt32(sc);
                if (row.TryGetValue("StatusMsg", out var sm)) msg = sm?.ToString();
            }

            // every set EXCEPT the trailing status set is its own dataset (preserved separately)
            var dataSets = sets.Count > 1
                ? sets.Take(sets.Count - 1).ToList()
                : new List<List<dynamic>>();

            return (dataSets, code, msg);
        }
    }
}
