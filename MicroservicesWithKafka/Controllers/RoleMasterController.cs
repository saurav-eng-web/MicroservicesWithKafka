using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class RoleMasterController : ControllerBase
    {
        private readonly IRoleMasterService _service;

        public RoleMasterController(IRoleMasterService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) =>
            Ok(await _service.GetRoleByIdAsync(id));

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllRolesAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleMaster model) =>
            Ok(await _service.CreateRoleAsync(model));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RoleMaster model) =>
            Ok(await _service.UpdateRoleAsync(model));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _service.DeleteRoleAsync(id));
    }
}
