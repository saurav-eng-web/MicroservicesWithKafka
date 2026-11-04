using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/dashboard")]
    [ApiController]
    public class LogisticDashboardController : ControllerBase
    {
        private readonly ILogisticDashboardService _service;
        private readonly ILogger<LogisticDashboardController> _logger;

        public LogisticDashboardController(ILogisticDashboardService service, ILogger<LogisticDashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Logistic dashboard counts (sp_LogisticDashBoard) + courier availability (unused dockets per courier).
        /// Replaces Dashboard.aspx.vb Page_Load.
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                return Ok(await _service.GetDashboardSummaryAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic dashboard summary failed");
                return StatusCode(500, new { message = "Failed to load dashboard. Please try again or contact support." });
            }
        }
    }
}
