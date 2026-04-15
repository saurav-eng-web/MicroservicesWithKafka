using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class SFReceivePORepository : ISFReceivePORepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public SFReceivePORepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<dynamic>> ManageSFReceivePOAsync(SFReceivePORequest request)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var parameters = new DynamicParameters();
            foreach (var prop in request.GetType().GetProperties())
            {
                parameters.Add("@" + prop.Name, prop.GetValue(request));
            }

            return await connection.QueryAsync<dynamic>(
                "SP_CRUD_SFReceivePOData",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
