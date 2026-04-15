using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class POReceiveRepository : IPOReceiveRepository
    {
        private readonly SQLConnectionService _sql;

        public POReceiveRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_PORecieve",
                request,
                commandType: CommandType.StoredProcedure
            );

            var sets = new List<List<dynamic>>();

            // Read ALL result sets
            while (!multi.IsConsumed)
            {
                var set = (await multi.ReadAsync<dynamic>()).AsList();
                sets.Add(set);
            }

            int statusCode = 500;
            string statusMsg = "Stored procedure did not return status";

            //  LAST result set = STATUS
            var statusSet = sets.LastOrDefault();

            if (statusSet != null && statusSet.Count > 0 &&
                statusSet[0] is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc) && sc != null)
                    statusCode = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm) && sm != null)
                    statusMsg = sm.ToString();
            }
            else
            {
                throw new Exception("No StatusCode/StatusMsg returned from SP_CRUD_PORecieve");
            }

            // All sets except last = DATA
            var data = sets.Count > 1 ? sets.Take(sets.Count - 1).SelectMany(x => x): Enumerable.Empty<dynamic>();

            return (data, statusCode, statusMsg);
        }
    }
}
