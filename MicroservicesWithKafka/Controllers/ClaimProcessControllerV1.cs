using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/claim-process-v1")]
    public class ClaimProcessControllerV1 : ControllerBase
    {
        private readonly IClaimProcessServiceV1 _service;

        public ClaimProcessControllerV1(IClaimProcessServiceV1 service)
        {
            _service = service;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            int userId = int.Parse(
                User.FindFirst("UserID")?.Value ?? "0"
            );

            var (data, code, msg) =
                await _service.ProcessAsync(payload, userId);

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
