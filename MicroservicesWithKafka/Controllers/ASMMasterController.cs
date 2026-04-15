using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class ASMMasterController : ControllerBase
    {
        private readonly IASMMasterService _asmService;

        public ASMMasterController(IASMMasterService asmService)
        {
            _asmService = asmService;
        }

        // ✅ Create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateASMMaster([FromBody] ASMMaster asm)
        {
            var created = await _asmService.CreateASMMasterAsync(asm);
            return CreatedAtAction(nameof(GetASMMasterById), new { id = created.ASM_ID }, created);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateASMMaster(int id, [FromBody] ASMMaster asm)
        {
            if (id != asm.ASM_ID)
                return BadRequest(new { message = "Id mismatch" });

            var result = await _asmService.UpdateASMMasterAsync(asm);
            if (!result)
                return NotFound(new { message = "Update failed. ASM not found." });

            // 🔥 Fetch the updated record and return it instead of NoContent()
            var updatedAsm = await _asmService.GetASMMasterByIdAsync(id);
            return Ok(updatedAsm);  // 200 OK with updated object
        }

        // ✅ Delete
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteASMMaster(int id)
        {
            var result = await _asmService.DeleteASMMasterAsync(id);
            if (!result)
                return NotFound(new { message = "Delete failed. ASM not found." });

            return Ok(new { message = $"ASM with ID {id} deleted successfully." });
        }


        // ✅ Get By Id
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetASMMasterById(int id)
        {
            var asm = await _asmService.GetASMMasterByIdAsync(id);
            if (asm == null)
                return NotFound(new { message = "ASM not found." });

            return Ok(asm);
        }

        // ✅ Get All
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<ASMMaster>>> GetAllASMMasters()
        {
            var list = await _asmService.GetAllASMMastersAsync();
            return Ok(list);
        }

        [HttpGet("GetAllRSM")]
        public async Task<ActionResult<List<ASMMaster>>> GetAllRSMMasters()
        {
            var list = await _asmService.GetAllASMMastersAsync();
            return Ok(list);
        }
    }
}
