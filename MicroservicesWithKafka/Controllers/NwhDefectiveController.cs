using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NwhDefectiveController : ControllerBase
    {
        private readonly INwhDefectiveService _service;

        public NwhDefectiveController(INwhDefectiveService service)
        {
            _service = service;
        }

        [HttpPost("Manage")]
        public async Task<IActionResult> Manage([FromBody] NwhDefectiveRequest request)
        {
            var result = await _service.ManageNwhDefectiveAsync(request);

            return Ok(new
            {
                statusCode = 200,
                message = "Operation executed successfully",
                data = result
            });
        }
    }
}
