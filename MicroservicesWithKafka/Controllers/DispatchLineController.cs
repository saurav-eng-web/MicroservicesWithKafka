using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [Authorize(Roles = "SF/SSD")]
    [ApiController]
    [Route("api/dispatch-line")]
    public class DispatchLineController : ControllerBase
    {
        private readonly IDispatchLineService _service;

        public DispatchLineController(IDispatchLineService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetDispatchLines(
            [FromBody] DispatchLineRequest request)
        {
            var (data, statusCode, message) =
                await _service.ProcessAsync(request);

            return StatusCode(statusCode, new
            {
                StatusCode = statusCode,
                StatusMessage = message,
                Count = data?.Count() ?? 0,
                Data = data
            });
        }
    }
}
