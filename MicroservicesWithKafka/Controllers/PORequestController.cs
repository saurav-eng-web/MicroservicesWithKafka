using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/po")]
public class PORequestController : ControllerBase
{
    private readonly IPORequestService _service;

    public PORequestController(IPORequestService service)
    {
        _service = service;
    }

    [HttpPost("request")]
    public async Task<IActionResult> CreatePO([FromBody] object payload)
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
