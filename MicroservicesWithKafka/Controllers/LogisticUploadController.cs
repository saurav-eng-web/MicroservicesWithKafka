using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/LogisticUpload")]
public class LogisticUploadController : ControllerBase
{
    private readonly ILogisticUploadService _service;

    public LogisticUploadController(ILogisticUploadService service)
    {
        _service = service;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        string uploadedBy = User.FindFirst("username")?.Value ?? string.Empty;

        var (data, count, code, msg) =
            await _service.ProcessAsync(file, uploadedBy);

        return StatusCode(code, new
        {
            StatusCode = code,
            StatusMsg = msg,
            Count = count,
            Data = data
        });
    }
}
