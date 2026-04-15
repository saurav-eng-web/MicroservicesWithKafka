using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace MicroservicesWithKafka.Services
{
    public class SQLConnectionService
    {
        private readonly string _connectionString;
        private readonly string _reportsConnection;

        public SQLConnectionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _reportsConnection = configuration.GetConnectionString("reportsConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public SqlConnection GetReportsConnection()
        {
            return new SqlConnection(_reportsConnection);
        }
    }
}
