using Dapper;
using Microsoft.Extensions.Logging;
using MicroservicesWithKafka.Services;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CheckHappyCodeRepository : ICheckHappyCodeRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<CheckHappyCodeRepository> _logger;

        public CheckHappyCodeRepository(SQLConnectionService sql, ILogger<CheckHappyCodeRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<dynamic> ExecuteAsync(string TK, string UserName)
        {
            try
            {
                using var conn = _sql.GetConnection();
                await conn.OpenAsync();

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_CRUD_CheckHappycode",
                    new { TK, UserName },
                    commandType: CommandType.StoredProcedure
                );

                return result; // <-- return exactly what DB returns
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "SP_CRUD_CheckHappycode failed");
                return new { StatusMsg = ex.Message, StatusCode = 500 };
            }
        }
    }
}
