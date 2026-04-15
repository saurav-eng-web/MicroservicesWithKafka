using Dapper;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class PaidWarrantyRepository : IPaidWarrantyRepository
    {
        private readonly SQLConnectionService _sql;

        public PaidWarrantyRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
    ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_PaidWarrantyProcess",
                request,
                commandType: CommandType.StoredProcedure
            );

            var sets = new List<List<dynamic>>();

            // Read all result sets
            while (!multi.IsConsumed)
            {
                sets.Add((await multi.ReadAsync<dynamic>()).AsList());
            }

            int statusCode = 500;
            string statusMsg = "Stored procedure did not return status";

            // The last result set should be the status result set
            var statusSet = sets.LastOrDefault();

            if (statusSet != null && statusSet.Count > 0)
            {
                var row = statusSet[0] as IDictionary<string, object>;

                // Ensure that the status is available
                if (row != null)
                {
                    if (row.TryGetValue("statuscode", out var sc))
                        statusCode = Convert.ToInt32(sc);

                    if (row.TryGetValue("statusmsg", out var sm))
                        statusMsg = sm?.ToString();
                }
            }
            else
            {
                throw new Exception("Stored procedure did not return StatusCode/StatusMsg");
            }

            // Get the data (all result sets except the last one)
            var data = sets.Count > 1
                ? sets.Take(sets.Count - 1).SelectMany(x => x)
                : Enumerable.Empty<dynamic>();

            return (data, statusCode, statusMsg);
        }

    }
}
