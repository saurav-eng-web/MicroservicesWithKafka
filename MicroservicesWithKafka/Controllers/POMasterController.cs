using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class POMasterController : ControllerBase
{
    private readonly IPOMasterService _service;

    public POMasterController(IPOMasterService service)
    {
        _service = service;
    }

    [HttpPost("Manage")]
    public async Task<IActionResult> ManagePOMaster([FromBody] POMasterRequest request)
    {
        var response = await _service.ManagePOMasterAsync(request);
        return Ok(response);
    }
}
