using Dapper;
using MicroservicesWithKafka.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class AdminStatusViewRepository : IAdminStatusViewRepository
    {
        private readonly SQLConnectionService _sql;

        public AdminStatusViewRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            // Maps cleanly to the multi-resultsets returned by SP_CRUD_Admin_StatusView
            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_Admin_StatusView",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
                resultSets.Add((await multi.ReadAsync<dynamic>()).AsList());

            int code = 500;
            string msg = "No status returned from Database";

            // Grab the last element containing corporate structure metrics (StatusCode, StatusMsg)
            var statusSet = resultSets.LastOrDefault();

            if (statusSet?.FirstOrDefault() is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm))
                    msg = sm?.ToString();
            }

            // If multiple grids were selected out (e.g. Type='open' which returns engineer metrics grid + standard metrics grid),
            // this gracefully bundles all prior grids before stripping the status set row.
            var data = resultSets.Count > 1
                ? resultSets.Take(resultSets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}