using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [ApiController]
    [Route("api/docket")]
    public class DocketController : ControllerBase
    {
        private readonly IDocketService _service;

        public DocketController(IDocketService service)
        {
            _service = service;
        }

        [HttpPost("get")]
        public async Task<IActionResult> GetDocket([FromBody] GetDocketNoRequest model)
        {
            var (data, code, msg) = await _service.GetDocketAsync(model);

            return StatusCode(code, new
            {
                StatusCode = code,
                StatusMsg = msg,
                Count = data?.Count() ?? 0,
                Data = data
            });
        }
    }
}
