using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/statusview")]
    [ApiController]
    public class LogisticStatusViewController : ControllerBase
    {
        // The 6 values the legacy dashboard links passed to StatusView.aspx?type=...
        private static readonly HashSet<string> AllowedTypes = new(StringComparer.Ordinal)
        {
            "DefectivePart", "DefectiveDOA", "GoodPart", "GoodMachine", "MismatchPart", "MismatchMachine"
        };

        private readonly ILogisticStatusViewService _service;
        private readonly ILogger<LogisticStatusViewController> _logger;

        public LogisticStatusViewController(ILogisticStatusViewService service, ILogger<LogisticStatusViewController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Dispatched challans for one dashboard tile. Replaces StatusView.aspx.vb Page_Load.
        /// type: DefectivePart | DefectiveDOA | GoodPart | GoodMachine | MismatchPart | MismatchMachine.
        /// Clicking a row opens GET api/logistic/dockets/history?type={type}&amp;challanNo={challanNo}
        /// (the API maps the code to the display name the history SP expects, as History.aspx did).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStatusView([FromQuery] string? type)
        {
            if (string.IsNullOrWhiteSpace(type) || !AllowedTypes.Contains(type.Trim()))
                return BadRequest(new { message = "type must be one of DefectivePart, DefectiveDOA, GoodPart, GoodMachine, MismatchPart, MismatchMachine." });

            try
            {
                return Ok(await _service.GetStatusViewAsync(type.Trim()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic status view {Type} failed", type);
                return StatusCode(500, new { message = "Failed to load status list. Please try again or contact support." });
            }
        }
    }
}
