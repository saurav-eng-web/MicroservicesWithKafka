using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/cancellation-requests")]
public class CancellationRequestController : ControllerBase
{
    private readonly ICancellationRequestService _service;

    public CancellationRequestController(ICancellationRequestService service)
    {
        _service = service;
    }

    [HttpPost("request")]
    public async Task<IActionResult> Cancel([FromBody] object payload)
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
