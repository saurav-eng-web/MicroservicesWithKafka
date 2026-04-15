using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NewCallRegisterController : ControllerBase
    {
        private readonly INewCallRegisterService _service;
        private readonly ILogger<NewCallRegisterController> _logger;

        public NewCallRegisterController(INewCallRegisterService service, ILogger<NewCallRegisterController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Save a new complaint entry.
        /// </summary>
        /// <param name="model">Complaint data</param>
        [HttpPost("SaveComplaint")]
        public async Task<IActionResult> SaveComplaint([FromBody] NewCallRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var returnNo = await _service.SaveComplaintAsync(model);

                if (string.IsNullOrWhiteSpace(returnNo))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { StatusCode = -1, StatusMsg = "Failed to save complaint" });
                }

                return Ok(new
                {
                    StatusCode = 1,
                    StatusMsg = "Complaint saved successfully",
                    ReturnNo = returnNo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving complaint");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { StatusCode = -99, StatusMsg = "Internal server error" });
            }
        }
    }
}
