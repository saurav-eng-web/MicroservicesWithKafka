using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardBucketController : ControllerBase
    {
        private readonly IDashboardBucketService _service;

        public DashboardBucketController(IDashboardBucketService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<DashboardBucket>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardBucket>> GetById(string id)
        {
            var bucket = await _service.GetByIdAsync(id);
            if (bucket == null)
                return NotFound(new { message = "Bucket not found" });

            return Ok(bucket);
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<List<DashboardBucket>>> GetByRole(int role)
        {
            return Ok(await _service.GetByRoleAsync(role));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DashboardBucket bucket)
        {
            await _service.CreateAsync(bucket);
            return CreatedAtAction(nameof(GetById), new { id = bucket.Id }, bucket);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DashboardBucket bucket)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Bucket not found" });

            await _service.UpdateAsync(id, bucket);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Bucket not found" });

            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("aggregation/{bid}/{role}")]
        public async Task<ActionResult<List<ComplaintAggregationResult>>> GetComplaintAggregation(int bid,int role)
        {
            var result = await _service.GetComplaintAggregationAsync(bid,role);
            return Ok(result);
        }

    }
}
