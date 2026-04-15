using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class ClaimProcessRepositoryV1 : IClaimProcessRepositoryV1
    {
        private readonly SQLConnectionService _sql;

        public ClaimProcessRepositoryV1(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_Claimprocessv1",
                request,
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
                resultSets.Add((await multi.ReadAsync<dynamic>()).AsList());

            int code = 500;
            string msg = "No status returned";

            var statusSet = resultSets.LastOrDefault();

            if (statusSet?.FirstOrDefault() is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm))
                    msg = sm?.ToString();
            }

            var data = resultSets.Count > 1
                ? resultSets.Take(resultSets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, code, msg);
        }
    }
}
