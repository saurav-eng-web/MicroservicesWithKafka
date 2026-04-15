using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintCustDetailController : ControllerBase
    {
        private readonly IComplaintCustDetailService _service;

        public ComplaintCustDetailController(IComplaintCustDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get complaint & customer detail by Ticket No
        /// </summary>
        /// <param name="ticketNo">Ticket number</param>
        /// <returns>ComplaintCustDetail</returns>
        [HttpGet("{ticketNo}")]
        public async Task<IActionResult> GetByTicketNo(string ticketNo)
        {
            if (string.IsNullOrWhiteSpace(ticketNo))
                return BadRequest(new { Message = "TicketNo is required" });

            var result = await _service.GetByTicketNoAsync(ticketNo);

            if (result == null)
                return NotFound(new { Message = $"Complaint with TicketNo {ticketNo} not found." });

            return Ok(result);
        }
    }
}
