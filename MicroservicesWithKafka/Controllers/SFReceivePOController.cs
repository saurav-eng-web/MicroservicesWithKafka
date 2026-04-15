using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SFReceivePOController : ControllerBase
    {
        private readonly ISFReceivePOService _service;

        public SFReceivePOController(ISFReceivePOService service)
        {
            _service = service;
        }

        [HttpPost("Manage")]
        public async Task<IActionResult> Manage([FromBody] SFReceivePORequest request)
        {
            if (string.IsNullOrEmpty(request.Action))
                return BadRequest(new { statusCode = 400, message = "Action is required" });

            var data = await _service.ManageSFReceivePOAsync(request);

            return Ok(new
            {
                statusCode = 200,
                message = "Request processed",
                data
            });
        }
    }
}
