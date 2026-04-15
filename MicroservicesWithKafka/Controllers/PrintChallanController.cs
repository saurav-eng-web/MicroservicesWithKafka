using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Service;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrintChallanController : ControllerBase
    {
        private readonly IPrintChallanService _service;

        public PrintChallanController(IPrintChallanService service)
        {
            _service = service;
        }

        [HttpPost("GetPrintChallanList")]
        public async Task<IActionResult> GetPrintChallanList([FromBody] PrintChallanList request)
        {
            if (string.IsNullOrWhiteSpace(request.SfId) || string.IsNullOrWhiteSpace(request.Type))
                return BadRequest(new { message = "SfId and Type are required." });

            var result = await _service.GetPrintChallanListAsync(request);

            // If error message returned from repository
            if (result.Any(r => r.Message?.StartsWith("Error:") == true))
            {
                return StatusCode(500, new { error = result.First().Message });
            }

            if (!result.Any())
                return NotFound(new { message = "No data found for the given parameters." });

            return Ok(result);
        }
    }
}
