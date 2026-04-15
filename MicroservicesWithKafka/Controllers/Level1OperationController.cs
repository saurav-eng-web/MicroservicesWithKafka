using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Level1OperationController : ControllerBase
    {
        private readonly ILevel1OperationService _service;

        public Level1OperationController(ILevel1OperationService service)
        {
            _service = service;
        }

        [HttpPost("UpdateLevel1Operation")]
        public async Task<IActionResult> UpdateLevel1(Level1OperationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TicketNo))
            {
                return BadRequest(new
                {
                    statusCode = 400,
                    message = "TicketNo is required"
                });
            }

            var response = await _service.Level1OperationAsync(request);

            return StatusCode(response.StatusCode, response);
        }
    }
}
