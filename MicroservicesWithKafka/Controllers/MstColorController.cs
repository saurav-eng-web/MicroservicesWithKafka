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
    public class MstColorController : ControllerBase
    {
        private readonly IMstColorService _service;

        public MstColorController(IMstColorService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TblMstColor color)
        {
            try
            {
                var created = await _service.CreateColorAsync(color);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TblMstColor color)
        {
            try
            {
                var success = await _service.UpdateColorAsync(color);
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
                var success = await _service.DeleteColorAsync(id);
                if (!success)
                    return NotFound();

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
                var result = await _service.GetColorByIdAsync(id);
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
                var result = await _service.GetAllColorsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
