using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class DOARepository : IDOARepository
    {
        private readonly SQLConnectionService _sql;

        public DOARepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_DOA_Process",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
            {
                resultSets.Add((await multi.ReadAsync<dynamic>()).AsList());
            }

            int code = 500;
            string msg = "Stored procedure did not return status";

            var statusSet = resultSets.LastOrDefault();

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
                throw new Exception("No StatusCode/StatusMsg returned from SP_CRUD_DOA_Process");
            }

            var data = resultSets.Count > 1
                ? resultSets.Take(resultSets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}
