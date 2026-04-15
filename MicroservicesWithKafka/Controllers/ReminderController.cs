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
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _service;

        public ReminderController(IReminderService service)
        {
            _service = service;
        }

        // ✅ CREATE
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Reminder model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.CreateReminderAsync(model);
            return Ok(new { Message = "Created", Id = id });
        }

        // ✅ UPDATE
        [HttpPut("updatebyid/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Reminder model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Remind_ID = id;
            var rows = await _service.UpdateReminderAsync(model);

            if (rows > 0)
                return Ok(new { Message = "Updated", Id = id });

            return NotFound(new { Message = "Reminder not found" });
        }

        // ✅ DELETE
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var rows = await _service.DeleteReminderAsync(id);

            if (rows > 0)
                return Ok(new { Message = "Deleted", Id = id });

            return NotFound(new { Message = "Reminder not found" });
        }

        // ✅ GET by ID
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var reminder = await _service.GetReminderByIdAsync(id);

            if (reminder == null)
                return NotFound(new { Message = "Reminder not found" });

            return Ok(reminder);
        }

        // ✅ GET by ID
        [HttpGet("Ticket/{TicketNo}")]
        public async Task<IActionResult> GetByTicketAsync(string TicketNo)
        {
            var reminder = await _service.GetReminderByTicketAsync(TicketNo);


            if (reminder == null)
                return NotFound(new { Message = "Reminder not found" });

            return Ok(reminder);
        }

        // ✅ GET all
        //[HttpGet("all")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var reminders = await _service.GetAllRemindersAsync();
        //    return Ok(reminders);
        //}
    }
}
