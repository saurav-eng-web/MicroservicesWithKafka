using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class ChallanRepository : IChallanRepository
    {
        private readonly SQLConnectionService _sql;

        public ChallanRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            ProcessChallanAsync(ProcessChallanRequest request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_ProcessChallan",
                new
                {
                    type = request.Type,
                    warehouse = request.Warehouse,
                    courierCode = request.CourierCode,
                    doc = request.Doc,
                    remark = request.Remark,
                    Dimensions = request.Dimensions,
                    weight = request.Weight,
                    box = request.Box,
                    ids = request.Ids,
                    IPaddress = request.IPAddress,
                    username = request.Username
                },
                commandType: CommandType.StoredProcedure
            );

            var resultSets = new List<List<dynamic>>();

            while (!multi.IsConsumed)
            {
                var set = (await multi.ReadAsync<dynamic>()).AsList();
                resultSets.Add(set);
            }

            // 🔹 Last result set is STATUS
            var statusSet = resultSets.LastOrDefault();

            int statusCode = 500;
            string statusMsg = "Unknown error";

            if (statusSet != null && statusSet.Count > 0)
            {
                var row = (IDictionary<string, object>)statusSet[0];

                statusCode =
                    row.ContainsKey("StatusCode") ? Convert.ToInt32(row["StatusCode"]) : 500;

                statusMsg = row.ContainsKey("statusMsg") ? row["statusMsg"]?.ToString(): row.ContainsKey("Message") ? row["Message"]?.ToString(): "No message";
            }

            // All other result sets are DATA
            var data =
                resultSets.Count > 1
                    ? resultSets.Take(resultSets.Count - 1).SelectMany(x => x)
                    : Enumerable.Empty<dynamic>();

            return (data, statusCode, statusMsg);
        }
    }
}
