using MicroservicesWithKafka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize(Roles = "SF/SSD")]
    [Route("api/[controller]")]
    [ApiController]
    public class AscReverseDashboardController : ControllerBase
    {
        private readonly IBranchReverseDashboardService _service;
        private readonly ILogger<AscReverseDashboardController> _logger;

        public AscReverseDashboardController(IBranchReverseDashboardService service, ILogger<AscReverseDashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            try
            {
                // The ASC is taken only from the signed login token
                if (!int.TryParse(User.FindFirst("SF_ID")?.Value, out var sfId) || sfId <= 0)
                    return StatusCode(403, LogisticResponse.Build(403, "No ASC is mapped to this user.", null));

                var (data, code, msg) = await _service.ProcessAsync(payload, DashboardScope.ForAsc(sfId));
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ASC reverse challan dashboard failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to load dashboard. Please try again or contact support.", null));
            }
        }
    }
}
