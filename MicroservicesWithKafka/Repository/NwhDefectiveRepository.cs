using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroservicesWithKafka.Repositories
{
    public class NwhDefectiveRepository : INwhDefectiveRepository
    {
        private readonly SQLConnectionService _sql;

        public NwhDefectiveRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<IEnumerable<NwhDefectiveResponse>> ManageNwhDefectiveAsync(NwhDefectiveRequest request)
        {
            using var con = _sql.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", request.Action);
            parameters.Add("@DefNwh_ID", request.DefNwh_ID);
            parameters.Add("@DefNwh_Partcode", request.DefNwh_Partcode);
            parameters.Add("@DefNwh_Description", request.DefNwh_Description);
            parameters.Add("@DefNwh_Qty", request.DefNwh_Qty);
            parameters.Add("@nwhcatid", request.nwhcatid);

            return await con.QueryAsync<NwhDefectiveResponse>(
                "SP_CRUD_NwhDefective",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
