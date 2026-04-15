using MicroservicesWithKafka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PODetailController : ControllerBase
{
    private readonly IPODetailService _service;

    public PODetailController(IPODetailService service)
    {
        _service = service;
    }

    [HttpPost("Manage")]
    public async Task<IActionResult> ManagePODetail([FromBody] PODetailRequest request)
    {
        var response = await _service.ManagePODetailAsync(request);
        return Ok(response);
    }
}
