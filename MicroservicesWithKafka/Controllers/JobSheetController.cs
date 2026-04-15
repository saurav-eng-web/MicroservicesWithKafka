using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class JobSheetController : ControllerBase
    {
        private readonly IJobSheetService _jobSheetService;

        public JobSheetController(IJobSheetService jobSheetService)
        {
            _jobSheetService = jobSheetService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessJobSheet([FromBody] JobSheetRequest request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Request body is required."
                });
            }

            try
            {
                var result = await _jobSheetService.ProcessJobSheetAsync(request);

                if (result > 0)
                {
                    return Ok(new
                    {
                        Status = "Success",
                        Message = "Job sheet processed successfully.",
                        RowsAffected = result
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Status = "Warning",
                        Message = "No rows were affected. Please check input values.",
                        RowsAffected = result
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    Message = "A database error occurred while processing the job sheet.",
                    Detail = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    Message = "An unexpected error occurred.",
                    Detail = ex.Message
                });
            }
        }
    }
}
