using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly SQLConnectionService _sql;

        public TicketRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_TicketMaster",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
            {
                var set = (await multi.ReadAsync<dynamic>()).ToList();
                resultSets.Add(set);
            }

            int code = 500;
            string msg = "No status returned";

            // ✅ LAST RESULTSET = STATUS
            var statusSet = resultSets.LastOrDefault();

            if (statusSet?.FirstOrDefault() is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm))
                    msg = sm?.ToString();
            }

            // ✅ ALL BEFORE LAST = DATA
            var data = resultSets.Count > 1
                ? resultSets.Take(resultSets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}