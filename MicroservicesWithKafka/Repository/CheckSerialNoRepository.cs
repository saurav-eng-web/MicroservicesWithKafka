using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class CheckSerialNoRepository : ICheckSerialNoRepository
    {
        private readonly SQLConnectionService _sql;

        public CheckSerialNoRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_checkserialno",
                request,
                commandType: CommandType.StoredProcedure
            );

            var sets = new List<List<dynamic>>();

            // Read ALL result sets
            while (!multi.IsConsumed)
            {
                sets.Add((await multi.ReadAsync<dynamic>()).AsList());
            }

            int code = 500;
            string msg = "Stored procedure did not return status";

            // LAST result set = STATUS
            var statusSet = sets.LastOrDefault();

            if (statusSet != null &&
                statusSet.Count > 0 &&
                statusSet[0] is IDictionary<string, object> row)
            {
                if (row.TryGetValue("statuscode", out var sc) ||
                    row.TryGetValue("StatusCode", out sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("statusmsg", out var sm) ||
                    row.TryGetValue("StatusMsg", out sm))
                    msg = sm?.ToString();
            }
            else
            {
                throw new Exception(
                    "No StatusCode/StatusMsg returned from SP_CRUD_checkserialno");
            }

            // DATA = all result sets except last
            var data = sets.Count > 1
                ? sets.Take(sets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}
