using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]  // 🔒 Secure all endpoints
    [ApiController]
    [Route("api/asc-pending-challan")]
    public class AscPendingChallanController : ControllerBase
    {
        private readonly IAscPendingChallanService _service;

        public AscPendingChallanController(IAscPendingChallanService service)
        {
            _service = service;
        }

        [HttpPost("list")]
        public async Task<IActionResult> List(
            [FromBody] AscPendingChallanRequest request)
        {
            var (data, statusCode, statusMessage) =
                await _service.ProcessAsync(request.Id, request.Type);

            return StatusCode(statusCode, new
            {
                StatusCode = statusCode,
                StatusMessage = statusMessage,
                Count = data.Count(),
                Data = data
            });
        }
    }
}
