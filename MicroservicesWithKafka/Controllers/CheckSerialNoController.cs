using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/CheckSerialNo")]   
public class CheckSerialNoController : ControllerBase
{
    private readonly ICheckSerialNoService _service;

    public CheckSerialNoController(ICheckSerialNoService service)
    {
        _service = service;
    }

    [HttpPost("CheckSerial")]   
    public async Task<IActionResult> CheckSerial([FromBody] object payload)
    {
        var (data, code, msg) =
            await _service.ProcessAsync(payload);

        return StatusCode(code, new
        {
            StatusCode = code,
            StatusMsg = msg,
            Count = data?.Count() ?? 0,
            Data = data
        });
    }
}
