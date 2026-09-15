using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/TDDOAPickup")]
public class TDDOAPickupController : ControllerBase
{
    private readonly ITDDOAPickupService _service;

    public TDDOAPickupController(ITDDOAPickupService service)
    {
        _service = service;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] object payload)
    {
        string userId = User.FindFirst("UserID")?.Value ?? "0";

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
