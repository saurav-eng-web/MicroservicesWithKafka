using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/logistic/challans")]
    [ApiController]
    public class LogisticChallanController : ControllerBase
    {
        private readonly ILogisticChallanService _service;
        private readonly ILogger<LogisticChallanController> _logger;

        public LogisticChallanController(ILogisticChallanService service, ILogger<LogisticChallanController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private IActionResult ServerError(Exception ex, string action)
        {
            _logger.LogError(ex, "Logistic challan {Action} failed", action);
            return StatusCode(500, new { message = $"Failed to {action}. Please try again or contact support." });
        }

        /// <summary>Month 1-12 and a sensible year, as offered by the legacy dropdowns.</summary>
        private static string? ValidatePeriod(ChallanSearchRequestViewModel request)
        {
            if (request.Month < 1 || request.Month > 12)
                return "month must be between 1 and 12.";
            if (request.Year < 2000 || request.Year > DateTime.Now.Year + 1)
                return "year is invalid.";
            return null;
        }

        private static string? ValidateAscRequest(ChallanSearchRequestViewModel request)
        {
            if (string.IsNullOrWhiteSpace(request.AscName))
                return "ascName is required.";
            return ValidatePeriod(request);
        }

        /// <summary>
        /// Challan list for the Logistic "Defective Challan" page.
        /// Replaces Logistic/PrintDefectiveChallan.aspx.vb: PrintChallanList(@SfId, @Type, @Month, @Year).
        /// challanType: Defective (default) | DOA | GoodSpare | GoodFG | MisFG | MisSpare.
        /// sfId: ASC_MASTER.ASCINTCODE from GET asclist?role=LOG.
        /// </summary>
        [HttpGet("defective")]
        public async Task<IActionResult> GetDefectiveChallans([FromQuery] ChallanSearchRequestViewModel request)
        {
            if (string.IsNullOrWhiteSpace(request.SfId))
                return BadRequest(new { message = "sfId is required (select a service center)." });
            var periodError = ValidatePeriod(request);
            if (periodError != null)
                return BadRequest(new { message = periodError });

            var spType = _service.ResolvePrintChallanType(request.ChallanType);
            if (spType == null)
                return BadRequest(new { message = "challanType must be one of Defective, DOA, GoodSpare, GoodFG, MisFG, MisSpare." });

            try
            {
                var result = await _service.GetDefectiveChallansAsync(request.SfId.Trim(), spType, request.Month, request.Year);
                return Ok(result);
            }
            catch (Exception ex) { return ServerError(ex, "load challans"); }
        }

        /// <summary>
        /// DOA defective challans for an ASC / month / year.
        /// Replaces Logistic/PrintDOADefectiveChallan.aspx.vb.
        /// </summary>
        [HttpGet("doa")]
        public async Task<IActionResult> GetDOADefectiveChallans([FromQuery] ChallanSearchRequestViewModel request)
        {
            var error = ValidateAscRequest(request);
            if (error != null) return BadRequest(new { message = error });
            try
            {
                return Ok(await _service.GetDOADefectiveChallansAsync(request));
            }
            catch (Exception ex) { return ServerError(ex, "load DOA challans"); }
        }

        /// <summary>
        /// Good part challans (Tbl_GudPOMaster) for an ASC / month / year.
        /// Replaces Logistic/PrintGoodChallan.aspx.vb. Returns gudNo, gudDate, gudDocNo, gudRemark.
        /// </summary>
        [HttpGet("good")]
        public async Task<IActionResult> GetGoodChallans([FromQuery] ChallanSearchRequestViewModel request)
        {
            var error = ValidateAscRequest(request);
            if (error != null) return BadRequest(new { message = error });
            try
            {
                return Ok(await _service.GetGoodChallansAsync(request));
            }
            catch (Exception ex) { return ServerError(ex, "load good challans"); }
        }

        /// <summary>
        /// Good machine challans for an ASC / month / year.
        /// Replaces Logistic/PrintGoodChallanMachine.aspx.vb.
        /// </summary>
        [HttpGet("goodmachine")]
        public async Task<IActionResult> GetGoodMachineChallans([FromQuery] ChallanSearchRequestViewModel request)
        {
            var error = ValidateAscRequest(request);
            if (error != null) return BadRequest(new { message = error });
            try
            {
                return Ok(await _service.GetGoodMachineChallansAsync(request));
            }
            catch (Exception ex) { return ServerError(ex, "load good machine challans"); }
        }

        /// <summary>
        /// Mismatch part challans for an ASC / month / year.
        /// Replaces Logistic/PrintMisChallan.aspx.vb.
        /// </summary>
        [HttpGet("mis")]
        public async Task<IActionResult> GetMisChallans([FromQuery] ChallanSearchRequestViewModel request)
        {
            var error = ValidateAscRequest(request);
            if (error != null) return BadRequest(new { message = error });
            try
            {
                return Ok(await _service.GetMisChallansAsync(request));
            }
            catch (Exception ex) { return ServerError(ex, "load mismatch challans"); }
        }

        /// <summary>
        /// Mismatch machine (FG) challans for an SF / month / year.
        /// Replaces Logistic/PrintMisChallanFG.aspx.vb, which used the logged-in SF (Session "sidfic").
        /// A user whose token carries an SF_ID is always limited to that SF; other Logistic/NWH users
        /// must pass sfId (ASCINTCODE).
        /// </summary>
        [HttpGet("misfg")]
        public async Task<IActionResult> GetMisChallanFG([FromQuery] ChallanSearchRequestViewModel request)
        {
            var claimSf = User.FindFirst("SF_ID")?.Value;
            if (int.TryParse(claimSf, out var sf) && sf > 0)
                request.SfId = claimSf;

            if (string.IsNullOrWhiteSpace(request.SfId))
                return BadRequest(new { message = "sfId is required (select a service center)." });
            var periodError = ValidatePeriod(request);
            if (periodError != null) return BadRequest(new { message = periodError });

            try
            {
                return Ok(await _service.GetMisChallanFGAsync(request));
            }
            catch (Exception ex) { return ServerError(ex, "load mismatch machine challans"); }
        }

        /// <summary>
        /// SF / ASC dropdown. role=LOG -> ASClist(@Role='LOG') as on the legacy Defective Challan page;
        /// no role -> full ASC_MASTER list as on the legacy DOA / Good / Mis pages.
        /// Returns [{ ascIntCode, ascName }].
        /// </summary>
        [HttpGet("asclist")]
        public async Task<IActionResult> GetAscList([FromQuery] string? role)
        {
            try
            {
                return Ok(await _service.GetAscListAsync(role));
            }
            catch (Exception ex) { return ServerError(ex, "load service center list"); }
        }
    }
}
