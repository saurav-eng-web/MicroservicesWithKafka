using Dapper;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class JobnOtherDetailNewRepository : IJobnOtherDetailNewRepository
    {
        private readonly SQLConnectionService _sql;
        private readonly ILogger<JobnOtherDetailNewRepository> _logger;

        public JobnOtherDetailNewRepository(
            SQLConnectionService sql,
            ILogger<JobnOtherDetailNewRepository> logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetAsync(string ticketNo, string type)
        {
            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            var parameters = new
            {
                ticket_no = ticketNo,
                type = type
            };

            var data = await conn.QueryAsync<dynamic>(
                "SP_CRUD_JobnOtherDetailNew",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (data != null && data.Any())
            {
                return (data, 200, "Success");
            }

            return (Enumerable.Empty<dynamic>(), 404, "No record found");
        }
    }
}

