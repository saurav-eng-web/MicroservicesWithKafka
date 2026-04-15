using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class HappycodeJobCloseCheckRepository
        : IHappycodeJobCloseCheckRepository
    {
        private readonly SQLConnectionService _sql;

        public HappycodeJobCloseCheckRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_happycodeJobCloseCheck",request,commandType: CommandType.StoredProcedure
            );

            var sets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
                sets.Add((await multi.ReadAsync<dynamic>()).AsList());

            int code = 500;
            string msg = "No status returned from stored procedure";

            var statusSet = sets.LastOrDefault();

            if (statusSet != null &&
                statusSet.Count > 0 &&
                statusSet[0] is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm))
                    msg = sm?.ToString();
            }
            else
            {
                throw new Exception(
                    "SP_CRUD_happycodeJobCloseCheck did not return status");
            }

            var data = sets.Count > 1
                ? sets.Take(sets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}
