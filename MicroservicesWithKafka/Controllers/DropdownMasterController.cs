using MicroservicesWithKafka.Models.Requests;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DropdownMasterController : ControllerBase
    {
        private readonly IDropdownMasterService _service;

        public DropdownMasterController(IDropdownMasterService service)
        {
            _service = service;
        }

        [HttpPost("GetDropdown")]
        public async Task<IActionResult> GetDropdown([FromBody] DropdownRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Type))
                return BadRequest(new { message = "Dropdown type is required." });

            var data = await _service.GetDropdownAsync(request);

            if (data == null || !data.Any())
                return NotFound(new { message = "No data found for the specified type." });

            return Ok(new
            {
                statusCode = 200,
                message = "Dropdown data fetched successfully.",
                data
            });
        }
    }
}
