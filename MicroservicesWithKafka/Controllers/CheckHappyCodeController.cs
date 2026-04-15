using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CheckHappyCodeController : ControllerBase
    {
        private readonly ICheckHappyCodeService _service;

        public CheckHappyCodeController(ICheckHappyCodeService service)
        {
            _service = service;
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] CheckHappyCodeRequest model)
        {
            var result = await _service.CheckAsync(model.TK, model.UserName);

            // Return exactly what SQL returned
            return Ok(result);
        }
    }
}
