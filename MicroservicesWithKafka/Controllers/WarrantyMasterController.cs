using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class WarrantyMasterController : ControllerBase
    {
        private readonly IWarrantyMasterService _service;

        public WarrantyMasterController(IWarrantyMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"WarrantyMaster with ID {id} not found." });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarrantyMaster warranty)
        {
            if (warranty == null)
                return BadRequest("Invalid data.");

            var newId = await _service.AddAsync(warranty);
            return Ok(new { message = "WarrantyMaster created successfully.", id = newId });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] WarrantyMaster warranty)
        {
            if (warranty == null || id != warranty.Warranty_ID)
                return BadRequest("Invalid data.");

            var updatedId = await _service.UpdateAsync(warranty);
            return Ok(new { message = "WarrantyMaster updated successfully.", id = updatedId });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deletedId = await _service.DeleteAsync(id);
            return Ok(new { message = "WarrantyMaster deleted successfully.", id = deletedId });
        }
    }
}
