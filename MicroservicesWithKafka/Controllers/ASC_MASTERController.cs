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
    public class ASC_MASTERController : ControllerBase
    {
        private readonly IASC_MASTERService _ascService;

        public ASC_MASTERController(IASC_MASTERService ascService)
        {
            _ascService = ascService;
        }

        // CREATE
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ASC_MASTER asc)
        {
            if (asc == null)
                return BadRequest(new { message = "ASC data is required" });

            var created = await _ascService.CreateASCAsync(asc);
            return Ok(created);
        }

        // UPDATE
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] ASC_MASTER asc)
        {
            if (asc == null)
                return BadRequest(new { message = "ASC data is required" });

            var updated = await _ascService.UpdateASCAsync(asc);
            if (!updated)
                return NotFound(new { message = "ASC update failed or not found" });

            return Ok(new { message = "ASC updated successfully" });
        }

        // DELETE
        [HttpDelete("delete/{ascIntCode}")]
        public async Task<IActionResult> Delete(long ascIntCode)
        {
            var deleted = await _ascService.DeleteASCAsync(ascIntCode);
            if (!deleted)
                return NotFound(new { message = "ASC not found or delete failed" });

            return Ok(new { message = "ASC deleted successfully" });
        }

        // GET by ID
        [HttpGet("{ascIntCode}")]
        public async Task<IActionResult> GetById(long ascIntCode)
        {
            var asc = await _ascService.GetASCByIdAsync(ascIntCode);
            if (asc == null)
                return NotFound(new { message = "ASC not found" });

            return Ok(asc);
        }

        // GET by getbyasccode
        [HttpGet("asccode/{asccode}")]
        public async Task<IActionResult> GetByasccode(string asccode)
        {
            var asc = await _ascService.GetASCByasccodeAsync(asccode);
            if (asc == null)
                return NotFound(new { message = "ASC not found" });

            return Ok(asc);
        }

        // GET all
        [HttpGet("all")]
        public async Task<ActionResult<List<ASC_MASTER>>> GetAll()
        {
            var list = await _ascService.GetAllASCsAsync();
            return Ok(list);
        }

        [HttpGet("role/{role}/{id}")]
        public async Task<ActionResult<List<ASC_MASTER>>> GetByRole(string role, int id)
        {
            var list = await _ascService.GetAscByRoleAsync(role, id);

            if (list == null || list.Count == 0)
                return NotFound(new { message = "No ASC records found for this role and id" });

            return Ok(list);
        }

    }
}
