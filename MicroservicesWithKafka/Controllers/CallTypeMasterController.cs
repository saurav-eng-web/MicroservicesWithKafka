using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Protects all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class CallTypeMasterController : ControllerBase
    {
        private readonly ICallTypeMasterService _service;

        public CallTypeMasterController(ICallTypeMasterService service)
        {
            _service = service;
        }

        // ✅ GET: api/calltypemaster
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // ✅ GET: api/calltypemaster/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { statusCode = 404, message = $"CallTypeMaster with ID {id} not found." });

            return Ok(result);
        }

        // ✅ POST: api/calltypemaster
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CallTypeMaster callType)
        {
            if (callType == null)
                return BadRequest(new { statusCode = 400, message = "Invalid request payload." });

            var newId = await _service.AddAsync(callType);
            return CreatedAtAction(nameof(GetById), new { id = newId },
                new { statusCode = 201, message = "CallTypeMaster created successfully.", id = newId });
        }

        // ✅ PUT: api/calltypemaster/{id}
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] CallTypeMaster callType)
        {
            if (callType == null || id != callType.CallType_ID)
                return BadRequest(new { statusCode = 400, message = "Invalid request data." });

            var updatedId = await _service.UpdateAsync(callType);
            return Ok(new { statusCode = 200, message = "CallTypeMaster updated successfully.", id = updatedId });
        }

        // ✅ DELETE: api/calltypemaster/{id}
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deletedId = await _service.DeleteAsync(id);
            if (deletedId == 0)
                return NotFound(new { statusCode = 404, message = $"CallTypeMaster with ID {id} not found." });

            return Ok(new { statusCode = 200, message = "CallTypeMaster deleted successfully.", id = deletedId });
        }
    }
}
