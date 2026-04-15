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
    public class LevelPendingHistoryController : ControllerBase
    {
        private readonly ILevelPendingHistoryService _service;

        public LevelPendingHistoryController(ILevelPendingHistoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LevelPendingHistory entity)
        {
            var newId = await _service.CreateAsync(entity);
            return Ok(new { NewID = newId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("ticket/{ticketNo}")]
        public async Task<IActionResult> GetByTicketNo(string ticketNo)
        {
            var result = await _service.GetByTicketNoAsync(ticketNo);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LevelPendingHistory entity)
        {
            if (id != entity.ID) return BadRequest("ID mismatch");
            await _service.UpdateAsync(entity);
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
