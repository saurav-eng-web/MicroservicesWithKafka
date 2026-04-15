using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly ISMSService _smsService;

        public SMSController(ISMSService smsService)
        {
            _smsService = smsService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSMS([FromForm] string message, [FromForm] string mobile, [FromForm] string ticketKey, [FromForm] string source)
        {
            bool success = await _smsService.SendSMSAsync(message, mobile, ticketKey, source);

            if (!success)
                return BadRequest("Failed to send SMS or save record.");

            return Ok("SMS sent and logged successfully.");
        }
    }
}
