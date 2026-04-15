using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CancellationController : ControllerBase
    {
        private readonly ICancellationService _cancellationService;

        public CancellationController(ICancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestCancellation([FromBody] Dictionary<string, object> payload)
        {
            if (payload == null)
                return BadRequest(new { message = "Request body is required" });

            string TICKET_NO = payload.ContainsKey("TICKET_NO") ? payload["TICKET_NO"].ToString() : null;
            string remark = payload.ContainsKey("remark") ? payload["remark"].ToString() : null;
            string type = payload.ContainsKey("type") ? payload["type"].ToString() : null;
            string action = payload.ContainsKey("action") ? payload["action"].ToString() : null;
            string remarkS = payload.ContainsKey("remarkS") ? payload["remarkS"].ToString() : null;
            int uid = payload.ContainsKey("uid") ? int.Parse(payload["uid"].ToString()) : 0;

            var success = await _cancellationService.HandleCancellationRequestAsync(TICKET_NO, remark, type, action, remarkS, uid);

            if (!success)
                return BadRequest(new { message = "Cancellation request failed" });

            return Ok(new { message = "Cancellation request processed successfully" });
        }
    }
}
