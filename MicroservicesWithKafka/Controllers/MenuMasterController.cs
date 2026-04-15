using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuMasterController : ControllerBase
    {
        private readonly IMenuMasterService _service;

        public MenuMasterController(IMenuMasterService service)
        {
            _service = service;
        }

        [HttpPost("manage")]
        public async Task<IActionResult> Manage([FromBody] MenuMasterRequest model)
        {
            if (string.IsNullOrEmpty(model.Action))
                return BadRequest(new
                {
                    status = false,
                    statusCode = 400,
                    message = "Action is required"
                });

            var (data, code, msg) = await _service.ProcessAsync(model);

            return StatusCode(code, new
            {
                status = code == 200,
                statusCode = code,
                message = msg,
                count = data.Count(),
                data
            });
        }
    }
}
