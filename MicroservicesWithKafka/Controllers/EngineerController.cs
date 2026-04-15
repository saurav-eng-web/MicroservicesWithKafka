using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EngineerController : ControllerBase
    {
        private readonly IEngineerService _service;

        public EngineerController(IEngineerService service)
        {
            _service = service;
        }

        [HttpPost("Manage")]
        public async Task<IActionResult> Manage([FromBody] SfEngineerRequest request)
        {
            var result = await _service.ManageEngineersAsync(request);

            return Ok(new SfEngineerResponse
            {
                StatusCode = 200,
                Message = "Engineer operation executed successfully",
                Data = result
            });
        }

        [HttpGet("Active/{sfid}")]
        public async Task<IActionResult> GetActive(long sfid)
        {
            var result = await _service.GetActiveEngineersBySFIDAsync(sfid);

            return Ok(new SfEngineerResponse
            {
                StatusCode = 200,
                Message = "Active engineers fetched successfully",
                Data = result
            });
        }
    }
}
