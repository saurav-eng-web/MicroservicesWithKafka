using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class ASCCreationController : ControllerBase
    {
        private readonly IASCCreationService _ascService;

        public ASCCreationController(IASCCreationService ascService)
        {
            _ascService = ascService;
        }

        // ✅ CREATE
        [HttpPost("create")]
        public async Task<ActionResult<ASCCreation>> CreateASCCreation([FromBody] ASCCreation asc)
        {
            var created = await _ascService.CreateASCCreationAsync(asc);
            return CreatedAtAction(nameof(GetASCCreationById), new { id = created.Id }, created);
        }

        // ✅ READ BY ID
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ASCCreation>> GetASCCreationById(int id)
        {
            var asc = await _ascService.GetASCCreationByIdAsync(id);
            if (asc == null)
                return NotFound(new { message = "ASC not found" });

            return Ok(asc);
        }

        // ✅ READ ALL
        // [HttpGet("getall")]
        //public async Task<ActionResult<IEnumerable<ASCCreation>>> GetAllASCCreations()
        //{
        //    var list = await _ascService.GetAllASCCreationsAsync();
        //    return Ok(list);
        //}

        // ✅ UPDATE
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateASCCreation(int id, [FromBody] ASCCreation asc)
        {
            if (id != asc.Id)
                return BadRequest(new { message = "Id mismatch" });

            var updated = await _ascService.UpdateASCCreationAsync(asc);

            if (updated == null)
                return NotFound(new { message = "Update failed. ASC not found." });

            return Ok(updated); // 👈 Return the updated record
        }


        // ✅ DELETE
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteASCCreation(int id)
        {
            var result = await _ascService.DeleteASCCreationAsync(id);
            if (!result)
                return NotFound(new { message = "Delete failed. ASC not found." });

            return NoContent();
        }
    }
}
