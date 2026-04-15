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
    public class ComplaintHistoryController : ControllerBase
    {
        private readonly IComplaintHistoryService _service;

        public ComplaintHistoryController(IComplaintHistoryService service)
        {
            _service = service;
        }

        // POST: api/ComplaintHistory
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComplaintHistory model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AddComplaintAsync(model);
            return Ok(new { Message = "Created", Id = id });
        }

        // PUT: api/ComplaintHistory/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] ComplaintHistory model)
        {
            //if (!ModelState.IsValid || id != model.Id)
            //    return BadRequest("Invalid data or mismatched ID.");

            var updated = await _service.UpdateComplaintAsync(model);
            if (!updated)
                return NotFound("Complaint not found");

            return Ok(new { Message = "Updated" });
        }

        // DELETE: api/ComplaintHistory/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _service.DeleteComplaintAsync(id);
            if (!deleted)
                return NotFound("Complaint not found");

            return Ok(new { Message = "Deleted" });
        }

        // GET: api/ComplaintHistory
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var complaints = await _service.GetAllComplaintsAsync();
        //    return Ok(complaints);
        //}

        // GET: api/ComplaintHistory/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var complaint = await _service.GetComplaintByIdAsync(id);
            if (complaint == null)
                return NotFound("Complaint not found");

            return Ok(complaint);
        }
        [HttpGet("history-by-ticket/{ticketNo}")]
        public async Task<IActionResult> GetHistoryByTicketNo(string ticketNo)
        {
            var history = await _service.GetAllComplaintsByTicketNoAsync(ticketNo);
            if (history == null || history.Count == 0)
                return NotFound("No complaint history found for this ticket");

            return Ok(history);
        }
    }
}
