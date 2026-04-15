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
    public class PartConsumptionController : ControllerBase
    {
        private readonly IPartConsumptionService _service;

        public PartConsumptionController(IPartConsumptionService service)
        {
            _service = service;
        }

        [HttpGet("ByTicket/{ticketNo}")]
        public async Task<IActionResult> GetByTicket(string ticketNo)
        {
            var result = await _service.GetByTicketNoAsync(ticketNo);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] PartConsumption pc)
        {
            var newId = await _service.InsertAsync(pc);
            return CreatedAtAction(nameof(GetById), new { id = newId }, pc);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PartConsumption pc)
        {
            if (id != pc.PC_ID) return BadRequest("ID mismatch");
            await _service.UpdateAsync(pc);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
