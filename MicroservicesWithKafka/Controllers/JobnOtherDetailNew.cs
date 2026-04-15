using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobnOtherDetailNewController : ControllerBase
    {
        private readonly IJobnOtherDetailNewService _service;

        public JobnOtherDetailNewController(IJobnOtherDetailNewService service)
        {
            _service = service;
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetJobDetails(
            [FromBody] JobnOtherDetailNewRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Ticket_No))
                return BadRequest("Ticket_No is required");

            var result = await _service.GetAsync(
                request.Ticket_No,
                string.IsNullOrWhiteSpace(request.Type) ? "Job" : request.Type
            );

            return Ok(
                //result.StatusCode == 0 ? 200 : result.StatusCode,
                new
                {
                    Data = result.Data,
                    StatusCode = result.StatusCode,
                    StatusMsg = result.StatusMsg
                }
            );
        }
    }
}
