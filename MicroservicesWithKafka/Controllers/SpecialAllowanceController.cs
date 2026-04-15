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
    [Route("api/[controller]")]
    public class SpecialAllowanceController : ControllerBase
    {
        private readonly ISpecialAllowanceService _service;

        public SpecialAllowanceController(ISpecialAllowanceService service)
        {
            _service = service;
        }

        [HttpPost("manage")]
        public async Task<IActionResult> Manage([FromBody] SpecialAllowanceRequest model)
        {
            // 🔑 READ CLAIMS
            string roleName = User.FindFirst("rolename")?.Value ?? string.Empty;
            string userName = User.FindFirst("username")?.Value ?? string.Empty;

            int bid = int.TryParse(User.FindFirst("Branch_ID")?.Value, out var b) ? b : 0;
            int rsmid = int.TryParse(User.FindFirst("Cat_MapID")?.Value, out var r) ? r : 0; // Cat_MapID = RSMID
            int asmid = int.TryParse(User.FindFirst("ASMID")?.Value, out var a) ? a : 0;
            int sfid = int.TryParse(User.FindFirst("SF_ID")?.Value, out var s) ? s : 0;

            // 🔑 INJECT INTO REQUEST MODEL
            model.BID ??= bid;
            model.RSMID ??= rsmid;
            model.ASMID ??= asmid;
            model.SFID ??= sfid;
            model.NSPRequestBy ??= userName;

            // 🔑 CALL SERVICE (NO CHANGE BELOW)
            var (data, code, msg) = await _service.ProcessAsync(model);

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
