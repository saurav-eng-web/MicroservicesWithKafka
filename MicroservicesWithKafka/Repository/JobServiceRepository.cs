//using MicroservicesWithKafka.Models;
//using Microsoft.Data.SqlClient;
//using System.Data;

//namespace MicroservicesWithKafka.Repository
//{
//    public class JobServiceRepository : IJobServiceRepository
//    {
//        private readonly IConfiguration _configuration;

//        public JobServiceRepository(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<JobService> CreateJobAsync(JobService job)
//        {
//            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            using var command = new SqlCommand("SP_CRUD_JobService", connection);
//            command.CommandType = CommandType.StoredProcedure;

//            // Required insert params
//            command.Parameters.AddWithValue("@Type", "INSERT");
//            command.Parameters.AddWithValue("@SERIAL_NO", job.SERIAL_NO ?? "");
//            command.Parameters.AddWithValue("@Mobile", job.Mobile ?? "");
//            command.Parameters.AddWithValue("@Custname", job.Custname ?? "");
//            command.Parameters.AddWithValue("@MODEL", job.MODEL ?? "");
//            command.Parameters.AddWithValue("@COMPLAINT_STATUS", job.COMPLAINT_STATUS ?? "");
//            command.Parameters.AddWithValue("@Complaint_SubStatus", job.Complaint_SubStatus ?? "");
//            command.Parameters.AddWithValue("@TDATE", job.TDATE ?? DateTime.Now);
//            command.Parameters.AddWithValue("@ASC_NAME", job.ASC_NAME ?? "");
//            command.Parameters.AddWithValue("@TICKET_NO", job.TICKET_NO ?? "");

//            // Add all other mandatory insert fields used in SP
//            // For brevity, only key fields shown above

//            await connection.OpenAsync();
//            var ticketKey = await command.ExecuteScalarAsync(); // SP must return scope_identity()

//            // Fetch created job details
//            using var detailCommand = new SqlCommand("SP_CRUD_JobService", connection);
//            detailCommand.CommandType = CommandType.StoredProcedure;
//            detailCommand.Parameters.AddWithValue("@Type", "SELECT_BY_ID");
//            detailCommand.Parameters.AddWithValue("@TICKET_KEY", Convert.ToDecimal(ticketKey));

//            using var reader = await detailCommand.ExecuteReaderAsync();
//            if (await reader.ReadAsync())
//            {
//                return new JobService
//                {
//                    TICKET_KEY = reader.GetDecimal(reader.GetOrdinal("TICKET_KEY")),
//                    TICKET_NO = reader["TICKET_NO"].ToString(),
//                    SERIAL_NO = reader["SERIAL_NO"].ToString(),
//                    Mobile = reader["Mobile"].ToString(),
//                    Custname = reader["Custname"].ToString(),
//                    MODEL = reader["MODEL"].ToString(),
//                    COMPLAINT_STATUS = reader["COMPLAINT_STATUS"].ToString(),
//                    Complaint_SubStatus = reader["Complaint_SubStatus"].ToString(),
//                    TDATE = reader.GetDateTime(reader.GetOrdinal("TDATE")),
//                    ASC_NAME = reader["ASC_NAME"].ToString()
//                };
//            }

//            return null;
//        }
//    }

//}


using MicroservicesWithKafka.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class JobServiceRepository : IJobServiceRepository
    {
        private readonly IConfiguration _configuration;

        public JobServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JobService> CreateJobAsync(JobService job)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var command = new SqlCommand("SP_CRUD_JobService", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Type", "INSERT");

            foreach (var prop in typeof(JobService).GetProperties())
            {
                var value = prop.GetValue(job) ?? DBNull.Value;
                // Avoid duplicating @Type since it is already added
                if ($"@{prop.Name}" != "@Type")
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", value);
                }
            }

            await connection.OpenAsync();
            var ticketKey = await command.ExecuteScalarAsync(); // SP must return SCOPE_IDENTITY()

            // Fetch created job details
            using var detailCommand = new SqlCommand("SP_CRUD_JobService", connection);
            detailCommand.CommandType = CommandType.StoredProcedure;
            detailCommand.Parameters.AddWithValue("@Type", "SELECT_BY_ID");
            detailCommand.Parameters.AddWithValue("@TICKET_KEY", Convert.ToDecimal(ticketKey));

            using var reader = await detailCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new JobService
                {
                    TICKET_KEY = reader.GetDecimal(reader.GetOrdinal("TICKET_KEY")),
                    TICKET_NO = reader["TICKET_NO"].ToString(),
                    SERIAL_NO = reader["SERIAL_NO"].ToString(),
                    Mobile = reader["Mobile"].ToString(),
                    Custname = reader["Custname"].ToString(),
                    MODEL = reader["MODEL"].ToString(),
                    COMPLAINT_STATUS = reader["COMPLAINT_STATUS"].ToString(),
                    Complaint_SubStatus = reader["Complaint_SubStatus"].ToString(),
                    TDATE = reader.GetDateTime(reader.GetOrdinal("TDATE")),
                    ASC_NAME = reader["ASC_NAME"].ToString()
                };
            }

            return null;
        }
    }
}
