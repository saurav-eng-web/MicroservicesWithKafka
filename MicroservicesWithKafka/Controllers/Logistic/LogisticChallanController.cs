using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/challans")]
    [ApiController]
    public class LogisticChallanController : ControllerBase
    {
        private readonly ILogisticChallanService _service;
        private readonly ILogger<LogisticChallanController> _logger;

        public LogisticChallanController(ILogisticChallanService service, ILogger<LogisticChallanController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            string? claimSfId = User.FindFirst("SF_ID")?.Value;
            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload, claimSfId);
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic challan process failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to load challans. Please try again or contact support.", null));
            }
        }
    }
}
