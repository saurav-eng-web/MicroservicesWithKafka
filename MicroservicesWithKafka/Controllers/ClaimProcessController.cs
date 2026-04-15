using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimProcessController : ControllerBase
    {
        private readonly IClaimProcessService _service;
        private readonly ILogger<ClaimProcessController> _logger;

        public ClaimProcessController(IClaimProcessService service, ILogger<ClaimProcessController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("select")]
        public async Task<IActionResult> Select([FromBody] ClaimProcessModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Action))
                return BadRequest(new { status = false, statusCode = 400, message = "Action required" });

            var (table1, table2, code, msg) = await _service.GetClaimDataAsync(model);

            return StatusCode(code, new
            {
                status = code == 200,
                statusCode = code,
                message = msg,
                table1Count = table1.Count(),
                table2Count = table2.Count(),
                table1,
                table2
            });
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] ClaimProcessModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Action))
                return BadRequest(new { status = false, statusCode = 400, message = "Action required" });

            var (code, msg) = await _service.UpdateClaim(model);

            return StatusCode(code, new
            {
                status = code == 200,
                statusCode = code,
                message = msg
            });
        }
    }
}
