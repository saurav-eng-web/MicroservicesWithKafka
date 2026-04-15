using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _service;
        public VisitController(IVisitService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> CreateVisit([FromBody] Visit visit)
        {
            var result = await _service.CreateVisitAsync(visit);
            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateVisit(long id, [FromBody] Visit visit)
        {
            if (id != visit.SlNo) return BadRequest("SlNo mismatch");
            var result = await _service.UpdateVisitAsync(visit);
            return Ok(result);
        }

        [HttpPost("by-ticket")]
        public async Task<IActionResult> UpdateVisitByTicket([FromBody] Visit visit)
        {
            var result = await _service.UpdateVisitByTicketAsync(visit);
            return Ok(result);
        }


        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteVisit(long id)
        {
            var result = await _service.DeleteVisitAsync(id);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetVisitById(long id)
        {
            var data = await _service.GetVisitByIdAsync(id);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVisits()
        {
            var data = await _service.GetAllVisitsAsync();
            return Ok(data);
        }

        [HttpGet("ticket/{ticketNo}")]
        public async Task<IActionResult> GetVisitsByTicketNo(string ticketNo)
        {
            var data = await _service.GetVisitsByTicketNoAsync(ticketNo); // unchanged
            return Ok(data);
        }
    }
}
