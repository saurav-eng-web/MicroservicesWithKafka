using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PrintController : ControllerBase
{
    private readonly IPrintDataService _service;

    public PrintController(IPrintDataService service)
    {
        _service = service;
    }

    [HttpPost("fetch")]
    public async Task<IActionResult> Fetch([FromBody] PrintDataRequest request)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.pono) ||
            string.IsNullOrWhiteSpace(request.Type) ||
            string.IsNullOrWhiteSpace(request.challantype))
        {
            return BadRequest(new
            {
                statusCode = 400,
                message = "Invalid request. All fields (pono, Type, challantype) are required."
            });
        }

        var result = await _service.FetchAsync(request);

        // Detect status from repository
        if (result.GetType().GetProperty("statusCode") != null)
        {
            int status = (int)result.GetType().GetProperty("statusCode").GetValue(result);
            return StatusCode(status, result);
        }

        // Success
        return Ok(new
        {
            statusCode = 200,
            message = "Print data fetched successfully",
            data = result
        });
    }
}
