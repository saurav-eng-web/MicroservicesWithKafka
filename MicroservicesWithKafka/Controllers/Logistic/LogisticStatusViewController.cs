using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticStatusViewController : ControllerBase
    {
        private readonly ILogisticStatusViewService _service;
        private readonly ILogger<LogisticStatusViewController> _logger;

        public LogisticStatusViewController(ILogisticStatusViewService service, ILogger<LogisticStatusViewController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload);
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic status view failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to load status list. Please try again or contact support.", null));
            }
        }
    }
}
