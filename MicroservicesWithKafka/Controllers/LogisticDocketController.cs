using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticDocketController : ControllerBase
    {
        private readonly ILogisticDocketService _service;
        private readonly ILogger<LogisticDocketController> _logger;

        public LogisticDocketController(ILogisticDocketService service, ILogger<LogisticDocketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            var userId = User.FindFirst("UserID")?.Value ?? string.Empty;
            var userName = User.FindFirst("username")?.Value ?? User.Identity?.Name ?? string.Empty;

            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload, userId, userName);
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic docket process failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to process docket request. Please try again or contact support.", null));
            }
        }
    }
}
