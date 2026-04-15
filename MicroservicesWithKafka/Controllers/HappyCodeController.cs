using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/happycode")]
public class HappyCodeController : ControllerBase
{
    private readonly IGenerateHappyCodeService _service;

    public HappyCodeController(IGenerateHappyCodeService service)
    {
        _service = service;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] object payload)
    {
        string userName = User.FindFirst("username")?.Value ??User.FindFirst("UserName")?.Value ??"SYSTEM";

        var (data, code, msg) =
            await _service.ProcessAsync(payload, userName);

        return StatusCode(code, new
        {
            StatusCode = code,
            StatusMsg = msg,
            Count = data?.Count() ?? 0,
            Data = data
        });
    }
}
