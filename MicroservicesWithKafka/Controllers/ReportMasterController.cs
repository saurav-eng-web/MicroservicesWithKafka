using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportMasterController : ControllerBase
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ReportMasterController(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public class ReportMaster
        {
            public int Id { get; set; }
            public string ReportAct { get; set; }
            public string ReportDis { get; set; }
            public string RolesAllowed { get; set; }
        }

        // ✅ GET ALL REPORTS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportMaster>>> GetAllReports()
        {
            var reports = new List<ReportMaster>();

            // Get a SqlConnection directly
            using (SqlConnection conn = _sqlConnectionService.GetConnection())
            {
                string query = "SELECT Id, ReportAct, ReportDis, RolesAllowed FROM ReportMaster";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reports.Add(new ReportMaster
                            {
                                Id = reader.GetInt32(0),
                                ReportAct = reader.GetString(1),
                                ReportDis = reader.GetString(2),
                                RolesAllowed = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return Ok(reports);
        }

        // ✅ GET REPORT BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReportMaster>> GetReportById(int id)
        {
            ReportMaster report = null;

            using (SqlConnection conn = _sqlConnectionService.GetConnection())
            {
                string query = "SELECT Id, ReportAct, ReportDis, RolesAllowed FROM ReportMaster WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            report = new ReportMaster
                            {
                                Id = reader.GetInt32(0),
                                ReportAct = reader.GetString(1),
                                ReportDis = reader.GetString(2),
                                RolesAllowed = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                    }
                }
            }

            if (report == null)
                return NotFound(new { message = $"Report with Id={id} not found." });

            return Ok(report);
        }
    }
}
