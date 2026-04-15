using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]  // 🔒 Secure all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class CallScheduleController : ControllerBase
    {
        private readonly ICallScheduleService _service;

        public CallScheduleController(ICallScheduleService service)
        {
            _service = service;
        }

        // --- POST: api/callschedule/create ---
        [HttpPost("create")]
        public async Task<IActionResult> Create(CallSchedule model)
        {
            var response = await _service.CreateAsync(model);

            // Use the StatusCode from the stored procedure (e.g., 201 Created)
            if (response.StatusCode.HasValue)
            {
                return StatusCode(response.StatusCode.Value, response);
            }

            // Fallback: This should ideally not be reached if SP works as designed
            return CreatedAtAction(nameof(GetById), new { id = response.ID }, response);
        }

        // --- PUT: api/callschedule/update ---
        [HttpPut("update")]
        public async Task<IActionResult> Update(CallSchedule model)
        {
            if (model.ID <= 0)
            {
                return BadRequest(new { StatusMsg = "ID is required for update." });
            }

            var response = await _service.UpdateAsync(model);

            // Use the StatusCode from the stored procedure (e.g., 204 No Content)
            if (response.StatusCode.HasValue)
            {
                // Return 200 OK with the message/code, or use the specific code (204)
                return StatusCode(response.StatusCode.Value, response);
            }

            return Ok(response);
        }

        // --- DELETE: api/callschedule/delete/5 ---
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);

            // Use the StatusCode from the stored procedure (e.g., 204 No Content)
            if (response.StatusCode.HasValue)
            {
                return StatusCode(response.StatusCode.Value, response);
            }

            return NoContent();
        }

        // --- GET: api/callschedule/all ---
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // --- GET: api/callschedule/5 ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        // --- GET: api/callschedule/5 ---
        [HttpGet("ticketno/{ticketno}")]
        public async Task<IActionResult> GetByticketno(string ticketno)
        {
            var data = await _service.GetByticketnoAsync(ticketno);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }
}
