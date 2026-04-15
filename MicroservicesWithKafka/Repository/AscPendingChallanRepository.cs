using Dapper;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class AscPendingChallanRepository : IAscPendingChallanRepository
    {
        private readonly SQLConnectionService _sql;

        public AscPendingChallanRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ExecuteAsync(int id, string type)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var multi = await conn.QueryMultipleAsync(
                "SP_CRUD_datalistingforExecption",
                new { id, Type = type },
                commandType: CommandType.StoredProcedure
            );

            // 1st result = DATA
            var data = await multi.ReadAsync<dynamic>();

            // 2nd result = STATUS FROM SP
            var status = await multi.ReadFirstAsync<dynamic>();

            return (
                data,
                (int)status.StatusCode,
                (string)status.StatusMessage
            );
        }
    }
}
