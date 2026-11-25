using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/cn")]
    [ApiController]
    public class LogisticCnController : ControllerBase
    {
        private readonly ILogisticCnService _service;
        private readonly ILogger<LogisticCnController> _logger;

        public LogisticCnController(ILogisticCnService service, ILogger<LogisticCnController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            var userId = User.FindFirst("UserID")?.Value ?? string.Empty;
            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload, userId);
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CN logistic process failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to process CN request. Please try again or contact support.", null));
            }
        }
    }
}
