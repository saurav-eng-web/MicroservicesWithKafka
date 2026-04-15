using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize] // 🔒 Applies to all actions below
[ApiController]
[Route("api/po-receive")]
public class POReceiveController : ControllerBase
{
    private readonly IPOReceiveService _service;

    public POReceiveController(IPOReceiveService service)
    {
        _service = service;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetPOReceive([FromBody] object payload)
    {
        string sfId = User.FindFirst("SF_ID")?.Value ?? "0";

        var (data, code, msg) = await _service.ProcessAsync(payload, sfId);

        return StatusCode(code, new
        {
            StatusCode = code,
            StatusMsg = msg,
            Count = data?.Count() ?? 0,
            Data = data
        });
    }
}
