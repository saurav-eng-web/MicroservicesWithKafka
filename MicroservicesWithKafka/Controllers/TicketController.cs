using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            var (data, code, msg) = await _service.ProcessAsync(payload);

            return StatusCode(code, new
            {
                statusCode = code,
                statusMsg = msg,
                count = data?.Count() ?? 0,
                data = data
            });
        }
    }
}