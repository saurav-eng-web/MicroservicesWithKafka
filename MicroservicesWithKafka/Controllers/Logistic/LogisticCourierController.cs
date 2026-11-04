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
    [Route("api/logistic/couriers")]
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

        private IActionResult ServerError(Exception ex, string action)
        {
            _logger.LogError(ex, "Logistic courier {Action} failed", action);
            return StatusCode(500, new { message = $"Failed to {action}. Please try again or contact support." });
        }

        /// <summary>
        /// Courier list (CourierMaster). Replaces CourierAdd.aspx.vb Page_Init grid binding.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCouriers()
        {
            try
            {
                return Ok(await _service.GetCouriersAsync());
            }
            catch (Exception ex) { return ServerError(ex, "load couriers"); }
        }

        /// <summary>
        /// Adds a courier after the duplicate-code check. Replaces CourierAdd.aspx.vb btnSave_Click.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCourier([FromBody] CourierViewModel courier)
        {
            if (courier == null)
                return BadRequest(new { message = "Invalid payload." });

            try
            {
                var result = await _service.AddCourierAsync(courier);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "save courier"); }
        }

        /// <summary>
        /// All courier pincode mappings (CourMapping). Replaces CourierMapping.aspx.vb MappingDownload_Click.
        /// </summary>
        [HttpGet("mappings")]
        public async Task<IActionResult> GetMappedCours()
        {
            try
            {
                return Ok(await _service.GetMappedCoursAsync());
            }
            catch (Exception ex) { return ServerError(ex, "load courier mappings"); }
        }

        /// <summary>
        /// Uploaded mapping rows that were not applied (CourMappingUpload isUpdate=0), including Action.
        /// Replaces CourierMapping.aspx.vb MappingNotDownload_Click.
        /// </summary>
        [HttpGet("mappings/unmapped")]
        public async Task<IActionResult> GetUnmappedCours()
        {
            try
            {
                return Ok(await _service.GetUnmappedCoursAsync());
            }
            catch (Exception ex) { return ServerError(ex, "load unmapped couriers"); }
        }

        /// <summary>
        /// Bulk uploads courier mapping rows (CourierCode, Pincode, Slab, Priority, Action).
        /// Replaces CourierMapping.aspx.vb btnUpload_Click: CourMappingUpload -> Set_CourierMapping.
        /// </summary>
        [HttpPost("mappings/upload")]
        public async Task<IActionResult> UploadCourierMapping([FromBody] List<CourierMappingUploadViewModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return BadRequest(new { message = "No rows provided." });

            try
            {
                var result = await _service.UploadCourierMappingAsync(rows);
                if (result.Success)
                    return Ok(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }
            catch (Exception ex) { return ServerError(ex, "upload courier mapping"); }
        }
    }
}
