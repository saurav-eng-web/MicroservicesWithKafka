using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    /// <summary>
    /// Logistic docket / dispatch reports. Replaces ReportLog.aspx.vb ("All Reports" in the Logistic menu).
    /// Each endpoint returns the SP rows as JSON; the frontend renders and exports them.
    /// </summary>
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/reports")]
    [ApiController]
    public class LogisticReportController : ControllerBase
    {
        private readonly ILogisticReportService _service;
        private readonly ILogger<LogisticReportController> _logger;

        public LogisticReportController(ILogisticReportService service, ILogger<LogisticReportController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private async Task<IActionResult> Run(Func<Task<IEnumerable<dynamic>>> query, string reportName)
        {
            try
            {
                return Ok(await query());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic report {Report} failed", reportName);
                return StatusCode(500, new { message = $"Failed to generate {reportName}. Please try again or contact support." });
            }
        }

        /// <summary>
        /// Validates the date range and normalises it to yyyy-MM-dd (unambiguous for SQL Server).
        /// Legacy only required one of the two dates; both are required here because the SP takes both.
        /// </summary>
        private static bool TryGetRange(string? fromDate, string? toDate, out string from, out string to, out string error)
        {
            from = to = error = string.Empty;
            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" };

            if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
            {
                error = "fromDate and toDate are required.";
                return false;
            }
            if (!DateTime.TryParseExact(fromDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f) ||
                !DateTime.TryParseExact(toDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
            {
                error = "Dates must be in yyyy-MM-dd format.";
                return false;
            }
            if (f > t)
            {
                error = "fromDate cannot be after toDate.";
                return false;
            }

            from = f.ToString("yyyy-MM-dd");
            to = t.ToString("yyyy-MM-dd");
            return true;
        }

        /// <summary>Available Docket Report — Report_AvailableDocket.</summary>
        [HttpGet("available-docket")]
        public Task<IActionResult> GetAvailableDocketReport()
            => Run(_service.GetAvailableDocketReportAsync, "Available Docket Report");

        /// <summary>Used Docket Report — Report_UsedDocket.</summary>
        [HttpGet("used-docket")]
        public Task<IActionResult> GetUsedDocketReport()
            => Run(_service.GetUsedDocketReportAsync, "Used Docket Report");

        /// <summary>Complete Docket Report — Report_CompleteDocket.</summary>
        [HttpGet("complete-docket")]
        public Task<IActionResult> GetCompleteDocketReport()
            => Run(_service.GetCompleteDocketReportAsync, "Complete Docket Report");

        /// <summary>Total Dispatch Report — Report_TotalDispatch(@fromdate, @todate).</summary>
        [HttpGet("total-dispatch")]
        public async Task<IActionResult> GetTotalDispatchReport([FromQuery] string? fromDate, [FromQuery] string? toDate)
        {
            if (!TryGetRange(fromDate, toDate, out var from, out var to, out var error))
                return BadRequest(new { message = error });
            return await Run(() => _service.GetTotalDispatchReportAsync(from, to), "Total Dispatch Report");
        }

        /// <summary>Total Dispatch Header Report — Report_TotalDispatch1(@fromdate, @todate).</summary>
        [HttpGet("total-dispatch-header")]
        public async Task<IActionResult> GetTotalDispatchHeaderReport([FromQuery] string? fromDate, [FromQuery] string? toDate)
        {
            if (!TryGetRange(fromDate, toDate, out var from, out var to, out var error))
                return BadRequest(new { message = error });
            return await Run(() => _service.GetTotalDispatchHeaderReportAsync(from, to), "Total Dispatch Header Report");
        }
    }
}
