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
    public class SymptomMasterController : ControllerBase
    {
        private readonly ISymptomMasterService _service;

        public SymptomMasterController(ISymptomMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SymptomMaster symptom)
        {
            var id = await _service.AddAsync(symptom);
            return CreatedAtAction(nameof(GetById), new { id }, symptom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, SymptomMaster symptom)
        {
            if (id != symptom.syid) return BadRequest("ID mismatch");

            await _service.UpdateAsync(symptom);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("ByProduct/{productId:long}")]
        public async Task<IActionResult> GetByProductId(long productId)
        {
            try
            {
                var result = await _service.GetByProductIdAsync(productId);
                if (result == null) return NotFound(new { message = $"No symptoms found for product {productId}." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", detail = ex.Message });
            }
        }

    }
}
