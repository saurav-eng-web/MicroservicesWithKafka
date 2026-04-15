using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class LocalityMasterController : ControllerBase
    {
        private readonly ILocalityMasterService _service;

        public LocalityMasterController(ILocalityMasterService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] LocalityMaster model)
        {
            var result = await _service.CreateLocalityAsync(model);
            return Ok(result);
        }


        [HttpGet("by-pincode/{pincode}")]
        public async Task<IActionResult> GetByPincode(string pincode)
        {
            var result = await _service.GetLocalitiesByPincodeAsync(pincode);
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] LocalityMaster model)
        {
            var result = await _service.UpdateLocalityAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _service.DeleteLocalityAsync(id);
            return Ok(result);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _service.GetLocalityByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllLocalitiesAsync();
            return Ok(result);
        }
    }
}
