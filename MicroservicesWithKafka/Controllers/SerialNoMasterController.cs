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
    public class SerialNoMasterController : ControllerBase
    {
        private readonly ISerialNoMasterService _service;

        public SerialNoMasterController(ISerialNoMasterService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<SerialNoMaster>> Create([FromBody] SerialNoMaster serial)
        {
            try
            {
                var created = await _service.CreateSerialNoAsync(serial);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(decimal id, [FromBody] SerialNoMaster serial)
        {
            if (id != serial.Id)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateSerialNoAsync(serial);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(decimal id)
        {
            var deleted = await _service.DeleteSerialNoAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SerialNoMaster>> GetById(decimal id)
        {
            var serial = await _service.GetSerialNoByIdAsync(id);
            if (serial == null)
                return NotFound();

            return Ok(serial);
        }

        [HttpGet]
        public async Task<ActionResult<List<SerialNoMaster>>> GetAll()
        {
            var list = await _service.GetAllSerialNosAsync();
            return Ok(list);
        }
        [HttpGet("by-serial/{serialNo}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNo)
        {
            var item = await _service.GetBySerialNumberAsync(serialNo);
            if (item == null)
                return NotFound("Serial number not found.");

            return Ok(item);
        }
    }
}
