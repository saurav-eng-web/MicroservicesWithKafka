using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class DispatchLineRepository : IDispatchLineRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<DispatchLineRepository> _logger;

        public DispatchLineRepository(
            SQLConnectionService sql,
            ILogger<DispatchLineRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(DispatchLineRequest request)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_Dispatchline",
                new
                {
                    sfid = request.Sfid,
                    whs = request.Whs,
                    type = request.Type
                },
                commandType: CommandType.StoredProcedure
            );

            // 1️⃣ First result set → DATA
            var data = await multi.ReadAsync<dynamic>();

            // 2️⃣ Second result set → STATUS
            var statusRow = await multi.ReadFirstAsync<dynamic>();

            int statusCode = statusRow.StatusCode;
            string message =
                statusRow.statusMsg ??
                statusRow.Message ??
                "Unknown response from database";

            return (data, statusCode, message);
        }
    }
}
