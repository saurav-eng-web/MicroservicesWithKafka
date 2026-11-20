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
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/dockets")]
    [ApiController]
    public class LogisticDocketController : ControllerBase
    {
        private readonly ILogisticDocketService _service;
        private readonly ILogger<LogisticDocketController> _logger;

        public LogisticDocketController(ILogisticDocketService service, ILogger<LogisticDocketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private IActionResult ServerError(Exception ex, string action)
        {
            _logger.LogError(ex, "Logistic docket {Action} failed", action);
            return StatusCode(500, new { message = $"Failed to {action}. Please try again or contact support." });
        }

        /// <summary>
        /// Returns dockets for the "Delete Unused Docket" grid, optionally filtered by docket number.
        /// Replaces DeleteDocket.aspx.vb BindGrid / BindGrid1 (DocMaster_Proc @Action='SELECT').
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDockets([FromQuery] string? search)
        {
            try
            {
                var result = await _service.GetDocketsAsync(search ?? string.Empty);
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load dockets"); }
        }

        /// <summary>
        /// Deletes a docket by id.
        /// Replaces DeleteDocket.aspx.vb OnRowDeleting (DocMaster_Proc @Action='DELETE', @CustomerId).
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDocket(int id)
        {
            try
            {
                var result = await _service.DeleteDocketAsync(id);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "delete docket"); }
        }

        /// <summary>
        /// Returns docket detail + pickup history for a challan.
        /// Replaces History.aspx.vb (sp_DocketDetails + PickUPhistory).
        /// type accepts the StatusView code (DefectivePart, DefectiveDOA, GoodPart, GoodMachine,
        /// MismatchPart, MismatchMachine) or the legacy display name ("Defective Part", ...).
        /// GET: api/logistic/dockets/history?type=DefectivePart&amp;challanNo=DEF-...
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetDocketDetail([FromQuery] string? type, [FromQuery] string? challanNo)
        {
            var challanType = _service.ResolveChallanType(type);
            if (challanType == null)
                return BadRequest(new { message = "A valid type is required (e.g. DefectivePart or 'Defective Part')." });
            if (string.IsNullOrWhiteSpace(challanNo))
                return BadRequest(new { message = "challanNo is required." });

            try
            {
                var result = await _service.GetDocketDetailAsync(challanType, challanNo);
                if (result == null)
                    return NotFound(new { message = "No result found" });
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load docket history"); }
        }

        /// <summary>
        /// Searches docket detail + pickup history.
        /// Replaces SearchHistory.aspx.vb Search_Click (sp_DocketDetailsSearch + PickUPhistory).
        /// Returns 404 "No result found" when nothing matches, as the legacy page did.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchDocket([FromQuery] string? type, [FromQuery] string? challanNo)
        {
            var challanType = _service.ResolveChallanType(type);
            if (challanType == null)
                return BadRequest(new { message = "Please select a valid type." });
            if (string.IsNullOrWhiteSpace(challanNo))
                return BadRequest(new { message = "Challan No is required." });

            try
            {
                var result = await _service.SearchDocketAsync(challanType, challanNo);
                if (result == null)
                    return NotFound(new { message = "No result found" });
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "search docket"); }
        }

        /// <summary>
        /// Bulk uploads docket change rows (ChallanNo, CourierCode, DocketNo, Type).
        /// Replaces DocketChange.aspx.vb: bulk copy -> stamp Userid (logged-in user) -> Set_updateDocketChange.
        /// </summary>
        [HttpPost("upload/change")]
        public async Task<IActionResult> UploadDocketChange([FromBody] List<DocketViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });

            // Legacy used Session("uidfic"); the JWT carries the same user id as the "UserID" claim.
            var userId = User.FindFirst("UserID")?.Value ?? string.Empty;

            try
            {
                var result = await _service.UploadDocketChangeAsync(rows, userId);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "upload docket change data"); }
        }

        /// <summary>
        /// Bulk uploads docket status rows (ChallanNo, Type, PickupDate, DeliveryDate, DocStatus).
        /// Replaces DocketChangeStatus.aspx.vb: bulk copy (Status -> DocStatus) -> Set_updateDocketChangeStatus.
        /// </summary>
        [HttpPost("upload/status")]
        public async Task<IActionResult> UploadDocketChangeStatus([FromBody] List<DocketViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });

            try
            {
                var result = await _service.UploadDocketChangeStatusAsync(rows);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "upload docket status"); }
        }

        /// <summary>
        /// Forward Logistic Docket Uploader: rows (ChallanNo, DocketNo, DocStatus, TransDate, UploadType, ChallanType).
        /// Bulk copy into challdocketstusuploder, then SP_CRUD_LogisticForwardDocket updates tbl_PORecieveHistory
        /// (In-Transit -> Docket_transit_Date, Delivered -> Docket_Delivery_date). A future TransDate is rejected.
        /// </summary>
        [HttpPost("upload/forward")]
        public async Task<IActionResult> UploadForwardDockets([FromBody] List<ForwardDocketUploadViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });

            // CreatedBy of the staging table is the login user NAME (the JWT "username" claim), not the numeric user id
            var userName = User.FindFirst("username")?.Value ?? User.Identity?.Name ?? string.Empty;

            try
            {
                var result = await _service.UploadForwardDocketsAsync(rows, userName);
                if (result.Success)
                    return Ok(new { message = result.Message, notApplied = result.NotApplied });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "upload forward docket data"); }
        }

        /// <summary>
        /// All dockets (DocMaster) for export.
        /// Replaces DocketUpload.aspx.vb DownloadDocket_Click.
        /// </summary>
        [HttpGet("downloads")]
        public async Task<IActionResult> GetDocketDownloads()
        {
            try
            {
                var result = await _service.GetDocketDownloadsAsync();
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load docket download data"); }
        }

        /// <summary>
        /// Dockets that failed to upload (DocMasterUpload Isupload=0).
        /// Replaces DocketUpload.aspx.vb DocketNot_Click.
        /// </summary>
        [HttpGet("not-uploaded")]
        public async Task<IActionResult> GetDocketsNotUploaded()
        {
            try
            {
                var result = await _service.GetDocketsNotUploadedAsync();
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load not-uploaded dockets"); }
        }

        /// <summary>
        /// Bulk uploads dockets (CourierCode, DocketNo).
        /// Replaces DocketUpload.aspx.vb bindgrid: DocMasterUpload(Courier, DocketNo) -> Set_updateDocketData.
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDockets([FromBody] List<DocketUploadRequestViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });

            try
            {
                var result = await _service.UploadDocketsAsync(rows);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "upload dockets"); }
        }
    }
}
