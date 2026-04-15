using Dapper;
using MicroservicesWithKafka.Services;
using System.Data;
using System.Text.Json;

namespace MicroservicesWithKafka.Repository
{
    public class SymptomCauseRepository : ISymptomCauseRepository
    {
        private readonly SQLConnectionService _sql;

        public SymptomCauseRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(object request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync("SP_CRUD_SymptomCause",request,commandType: CommandType.StoredProcedure);

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

            if (statusSet != null && statusSet.Count > 0 && statusSet[0] is IDictionary<string, object> row)
            {
                if (row.TryGetValue("StatusCode", out var sc))
                    code = Convert.ToInt32(sc);

                if (row.TryGetValue("StatusMsg", out var sm))
                    msg = sm?.ToString();
            }
            else
            {
                throw new Exception(
                    "No StatusCode/StatusMsg returned from SP_CRUD_SymptomCause");
            }

            // DATA = all sets except last
            //var data = sets.Count > 1 ? sets.Take(sets.Count - 1).SelectMany(x => x): Enumerable.Empty<dynamic>();

            //return (data, code, msg);
            // DATA = all sets except last
            var data = sets.Count > 1 ? sets.Take(sets.Count - 1).SelectMany(x => x): Enumerable.Empty<dynamic>();

            // FIX: Parse DataFlow JSON string BEFORE returning
            var parsedData = data.Select(row =>
            {
                var dict = (IDictionary<string, object>)row;

                if (dict.TryGetValue("DataFlow", out var df) && df is string json)
                {
                    dict["DataFlow"] = JsonSerializer.Deserialize<object>(json);
                }

                return dict;
            }).ToList();

            // RETURN CLEAN DATA
            return (parsedData, code, msg);


        }
    }
}
