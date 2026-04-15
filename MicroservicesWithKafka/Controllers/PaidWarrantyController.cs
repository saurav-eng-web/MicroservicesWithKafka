using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/paid-warranty")]
public class PaidWarrantyController : ControllerBase
{
    private readonly IPaidWarrantyService _service;

    public PaidWarrantyController(IPaidWarrantyService service)
    {
        _service = service;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] object payload)
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
