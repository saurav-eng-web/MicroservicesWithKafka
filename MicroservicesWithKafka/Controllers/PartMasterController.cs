using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class PartMasterController : ControllerBase
    {
        private readonly IPartMasterService _partMasterService;

        public PartMasterController(IPartMasterService partMasterService)
        {
            _partMasterService = partMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartMaster>>> GetAll()
        {
            var parts = await _partMasterService.GetAllPartsAsync();
            return Ok(parts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartMaster>> GetById(decimal id)
        {
            var part = await _partMasterService.GetPartByIdAsync(id);
            if (part == null)
                return NotFound();

            return Ok(part);
        }

        [HttpPost]
        public async Task<ActionResult<PartMaster>> Create(PartMaster part)
        {
            var created = await _partMasterService.CreatePartAsync(part);
            return CreatedAtAction(nameof(GetById), new { id = created.PartID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(decimal id, PartMaster part)
        {
            if (id != part.PartID)
                return BadRequest("Part ID mismatch");

            var updated = await _partMasterService.UpdatePartAsync(part);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            var deleted = await _partMasterService.DeletePartAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
