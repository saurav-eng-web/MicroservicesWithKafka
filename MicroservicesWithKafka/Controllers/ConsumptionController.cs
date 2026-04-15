using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/consumption")]
public class ConsumptionController : ControllerBase
{
    private readonly IConsumptionService _service;

    public ConsumptionController(IConsumptionService service)
    {
        _service = service;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] object payload)
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
