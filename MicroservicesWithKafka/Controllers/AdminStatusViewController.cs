using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminStatusViewController : ControllerBase
    {
        private readonly IAdminStatusViewService _service;

        public AdminStatusViewController(IAdminStatusViewService service)
        {
            _service = service;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            var (data, code, msg) = await _service.ProcessAsync(payload);

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