using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [ApiController]
    [Route("api/challan")]
    public class ChallanController : ControllerBase
    {
        private readonly IChallanService _service;

        public ChallanController(IChallanService service)
        {
            _service = service;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessChallan(
            [FromBody] ProcessChallanRequest request)
        {
            var (data, code, msg) = await _service.ProcessAsync(request);

            return StatusCode(code, new
            {
                StatusCode = code,
                StatusMsg = msg,
                Count = data?.Count() ?? 0,
                Data = data
            });
        }
    }
}
