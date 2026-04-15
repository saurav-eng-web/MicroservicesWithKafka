using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class ModelVariantController : ControllerBase
    {
        private readonly IModelVariantService _service;

        public ModelVariantController(IModelVariantService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ModelVariant variant)
        {
            try
            {
                var created = await _service.CreateModelVariantAsync(variant);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ModelVariant variant)
        {
            try
            {
                var success = await _service.UpdateModelVariantAsync(variant);
                if (!success)
                    return NotFound();

                return Ok(new { message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            try
            {
                var success = await _service.DeleteModelVariantAsync(id);
                if (!success)
                    return NotFound();

                //return NoContent(); // 204 - Success, no response body

                return Ok(new { message = "Deleted successfully", result = 1 });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(decimal id)
        {
            try
            {
                var result = await _service.GetModelVariantByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllModelVariantsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
