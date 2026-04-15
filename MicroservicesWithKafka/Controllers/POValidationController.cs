using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/po-validation")]
public class POValidationController : ControllerBase
{
    private readonly IPOValidationService _service;

    public POValidationController(IPOValidationService service)
    {
        _service = service;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] object payload)
    {
        string sfId = User.FindFirst("SF_ID")?.Value ?? "0";

        var (data, code, msg) =
            await _service.ProcessAsync(payload, sfId);

        return StatusCode(code, new
        {
            StatusCode = code,
            StatusMsg = msg,
            Count = data?.Count() ?? 0,
            Data = data
        });
    }
}
