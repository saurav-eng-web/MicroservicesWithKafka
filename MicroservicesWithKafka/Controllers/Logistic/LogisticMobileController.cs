using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    /// <summary>
    /// "Mobile" section of the legacy Logistic menu (UC/LogisticMenu.ascx):
    /// Mob/reportsm.aspx, Mob/ComplaintSearch.aspx (+ Mob/complainthistory.aspx) and
    /// Mob/PrintDefectiveChallanho.aspx (+ Mob/PdefInvoice.aspx -> PrintInvoiceDef_NSP.aspx).
    /// </summary>
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/mobile")]
    [ApiController]
    public class LogisticMobileController : ControllerBase
    {
        private readonly ILogisticMobileService _service;
        private readonly ILogisticChallanService _challanService;
        private readonly ILogger<LogisticMobileController> _logger;

        public LogisticMobileController(ILogisticMobileService service, ILogisticChallanService challanService,
                                        ILogger<LogisticMobileController> logger)
        {
            _service = service;
            _challanService = challanService;
            _logger = logger;
        }

        private string CurrentUserId() => User.FindFirst("UserID")?.Value ?? string.Empty;

        private IActionResult ServerError(Exception ex, string action)
        {
            _logger.LogError(ex, "Logistic mobile {Action} failed", action);
            return StatusCode(500, new { message = $"Failed to {action}. Please try again or contact support." });
        }

        // ---------------------------------------------------------------- Print challan (Mob)

        /// <summary>SF dropdown — ASClistm(@id='', @Role='LOG'). Returns the SP rows (asc_name / ASC_id).</summary>
        [HttpGet("asclist")]
        public async Task<IActionResult> GetAscList()
        {
            try { return Ok(await _service.GetAscListAsync()); }
            catch (Exception ex) { return ServerError(ex, "load service center list"); }
        }

        /// <summary>
        /// Mobile challan list — PrintChallanListm(@SfId, @Type, @Month, @Year).
        /// challanType: Defective | DOA | GoodSpare | GoodFG | MisFG | MisSpare (same dropdown as the legacy page).
        /// </summary>
        [HttpGet("challans")]
        public async Task<IActionResult> GetChallans([FromQuery] string? sfId, [FromQuery] string? challanType,
                                                     [FromQuery] int month, [FromQuery] int year)
        {
            if (string.IsNullOrWhiteSpace(sfId))
                return BadRequest(new { message = "sfId is required (select a service center)." });
            if (month < 1 || month > 12)
                return BadRequest(new { message = "month must be between 1 and 12." });
            if (year < 2000 || year > DateTime.Now.Year + 1)
                return BadRequest(new { message = "year is invalid." });
            var spType = _challanService.ResolvePrintChallanType(challanType);
            if (spType == null)
                return BadRequest(new { message = "challanType must be one of Defective, DOA, GoodSpare, GoodFG, MisFG, MisSpare." });

            try { return Ok(await _service.GetChallanListAsync(sfId.Trim(), spType, month, year)); }
            catch (Exception ex) { return ServerError(ex, "load mobile challans"); }
        }

        /// <summary>
        /// Mobile delivery challan print data — invoicedetail1m (header) + ChallanLinem (lines).
        /// type is the PrintChallanListm type value (Defective, DOA, goodspare, GoodFG, MisFG, MisSpare).
        /// </summary>
        [HttpGet("challans/print")]
        public async Task<IActionResult> GetChallanPrint([FromQuery] string? poNo, [FromQuery] string? type)
        {
            if (string.IsNullOrWhiteSpace(poNo))
                return BadRequest(new { message = "poNo is required." });
            var spType = _challanService.ResolvePrintChallanType(type);
            if (spType == null)
                return BadRequest(new { message = "type is invalid." });

            try
            {
                var result = await _service.GetChallanPrintAsync(poNo, spType);
                if (result.Header == null && !result.Lines.Any())
                    return NotFound(new { message = "No result found" });
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load challan print"); }
        }

        // ---------------------------------------------------------------- Reports (Mob/reportsm.aspx)

        /// <summary>
        /// Downloads a mobile report as .xlsx — reportsm(@fromdate, @todate, @type, @userid=logged-in user).
        /// Returns 204 when the report has no rows.
        /// </summary>
        [HttpPost("reports/download")]
        public async Task<IActionResult> DownloadReport([FromBody] MobileReportRequestViewModel request)
        {
            if (request == null || !_service.IsValidReportType(request.Type))
                return BadRequest(new { message = "Please select a valid report." });

            string[] formats = { "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" };
            if (!DateTime.TryParseExact(request.FromDate?.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var from) ||
                !DateTime.TryParseExact(request.ToDate?.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var to))
                return BadRequest(new { message = "From Date and To Date are required (yyyy-MM-dd)." });
            if (from > to)
                return BadRequest(new { message = "From Date cannot be after To Date." });

            try
            {
                var rows = (await _service.GetReportAsync(request.Type!, from.ToString("yyyy-MM-dd"), to.ToString("yyyy-MM-dd"), CurrentUserId()))
                           .Select(r => (IDictionary<string, object>)r).ToList();
                if (rows.Count == 0)
                    return NoContent();

                using var workbook = new XLWorkbook();
                var sheet = workbook.Worksheets.Add("Sheet1");
                var columns = rows[0].Keys.ToList();
                for (int c = 0; c < columns.Count; c++)
                {
                    var cell = sheet.Cell(1, c + 1);
                    cell.Value = columns[c];
                    cell.Style.Font.Bold = true;
                }
                for (int r = 0; r < rows.Count; r++)
                {
                    for (int c = 0; c < columns.Count; c++)
                    {
                        var value = rows[r][columns[c]];
                        var cell = sheet.Cell(r + 2, c + 1);
                        if (value is DateTime dt)
                        {
                            cell.Value = dt;
                            cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm:ss";
                        }
                        else
                        {
                            cell.Value = value?.ToString() ?? string.Empty;
                        }
                    }
                }
                sheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{request.Type}_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex) { return ServerError(ex, "generate mobile report"); }
        }

        // ---------------------------------------------------------------- Complaint search (Mob/ComplaintSearch.aspx)

        /// <summary>Mobile complaint search — Search_Customer25(@Text, @UserId, @ipaddress).</summary>
        [HttpGet("complaints/search")]
        public async Task<IActionResult> SearchComplaints([FromQuery] string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest(new { message = "Please enter a search value." });

            // Legacy: HTTP_X_FORWARDED_FOR, else REMOTE_ADDR
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
            if (string.IsNullOrWhiteSpace(ip))
                ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            try { return Ok(await _service.SearchComplaintsAsync(text, CurrentUserId(), ip)); }
            catch (Exception ex) { return ServerError(ex, "search complaints"); }
        }

        /// <summary>Mobile complaint history — sp_ReminderHistory6 + sp_GetSupportingDocumentmah.</summary>
        [HttpGet("complaints/{ticketKey}/history")]
        public async Task<IActionResult> GetComplaintHistory(string ticketKey)
        {
            if (string.IsNullOrWhiteSpace(ticketKey))
                return BadRequest(new { message = "ticketKey is required." });
            try { return Ok(await _service.GetComplaintHistoryAsync(ticketKey, CurrentUserId())); }
            catch (Exception ex) { return ServerError(ex, "load complaint history"); }
        }
    }
}
