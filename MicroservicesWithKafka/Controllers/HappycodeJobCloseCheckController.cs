using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]  // Secure all endpoints
[ApiController]
[Route("api/happycode-job-close-check")]
public class HappycodeJobCloseCheckController : ControllerBase
{
    private readonly IHappycodeJobCloseCheckService _service;

    public HappycodeJobCloseCheckController(IHappycodeJobCloseCheckService service)
    {
        _service = service;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] object payload)
    {
        // Explicit tuple typing (safe & clear)
        (IEnumerable<dynamic> Data, int StatusCode, string StatusMsg) result
            = await _service.ProcessAsync(payload);

        return StatusCode(result.StatusCode, new
        {
            StatusCode = result.StatusCode,
            StatusMsg = result.StatusMsg,
            Count = result.Data?.Count() ?? 0,
            Data = result.Data
        });
    }
}
