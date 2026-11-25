using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Roles = "Logistic1")]
    [Route("api/logistic/invoice-uploads")]
    [ApiController]
    public class LogisticInvoiceUploadController : ControllerBase
    {
        private readonly ILogisticInvoiceUploadService _service;
        private readonly ILogger<LogisticInvoiceUploadController> _logger;

        public LogisticInvoiceUploadController(ILogisticInvoiceUploadService service, ILogger<LogisticInvoiceUploadController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string CurrentUserName() => User.FindFirst("username")?.Value ?? User.Identity?.Name ?? string.Empty;

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload, CurrentUserName());
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic invoice upload failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to upload invoice data. Please try again or contact support.", null));
            }
        }
    }
}
