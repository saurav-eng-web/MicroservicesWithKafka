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
    public class LogisticCourierController : ControllerBase
    {
        private readonly ILogisticCourierService _service;
        private readonly ILogger<LogisticCourierController> _logger;

        public LogisticCourierController(ILogisticCourierService service, ILogger<LogisticCourierController> logger)
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
                _logger.LogError(ex, "Logistic courier process failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to process courier request. Please try again or contact support.", null));
            }
        }
    }
}
