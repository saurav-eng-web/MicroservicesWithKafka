using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cost")]
    public class GetCostController : ControllerBase
    {
        private readonly IGetCostService _service;

        public GetCostController(IGetCostService service)
        {
            _service = service;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckCost([FromBody] GetCostRequest model)
        {
            // READ CLAIMS
            int sf_id = int.TryParse(User.FindFirst("SF_ID")?.Value, out var s) ? s : 0;

            // Enforce SfId from token if not provided
            model.SfId ??= sf_id;

            var (data, code, msg) = await _service.ProcessAsync(model);

            return StatusCode(code, new
            {
                StatusCode = code,
                StatusMsg = msg,
                Data = data
            });
        }

    }
}
