using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    /// <summary>
    /// Logistic1 invoice uploaders. Replaces GST/LogisticInvoiceUploader.aspx.vb and
    /// GST/LogisticInvoiceRateUploader.aspx.vb, which were only in the Logistic1 menu (UC/Log1.ascx).
    /// </summary>
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

        // Legacy stamped CreatedBy with Session("UName"); the JWT carries it as the "username" claim.
        private string CurrentUserName() => User.FindFirst("username")?.Value ?? User.Identity?.Name ?? string.Empty;

        /// <summary>
        /// Logistic invoice PO rows (InvoiceNo, FromA, Shipto, TransporterName, DocketNo, VehicleType)
        /// -> LogisticInvoicePOUploader, CreatedBy = logged-in user name.
        /// </summary>
        [HttpPost("po")]
        public async Task<IActionResult> UploadInvoicePo([FromBody] List<LogisticInvoicePoUploadViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });
            try
            {
                var result = await _service.UploadInvoicePoAsync(rows, CurrentUserName());
                return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic invoice PO upload failed");
                return StatusCode(500, new { message = "Failed to upload invoice data. Please try again or contact support." });
            }
        }

        /// <summary>
        /// Logistic invoice rate rows (FromA, ToA, Transporter, VehicleType, Rate)
        /// -> LogisticInvoiceRateUploader, CreatedBy = logged-in user name.
        /// </summary>
        [HttpPost("rate")]
        public async Task<IActionResult> UploadInvoiceRate([FromBody] List<LogisticInvoiceRateUploadViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });
            try
            {
                var result = await _service.UploadInvoiceRateAsync(rows, CurrentUserName());
                return result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic invoice rate upload failed");
                return StatusCode(500, new { message = "Failed to upload rate data. Please try again or contact support." });
            }
        }
    }
}
