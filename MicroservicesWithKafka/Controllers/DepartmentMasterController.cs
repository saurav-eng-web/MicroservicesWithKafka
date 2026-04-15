using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly IDepartmentMasterService _departmentService;

        public DepartmentMasterController(IDepartmentMasterService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentMaster model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _departmentService.CreateAsync(model);
            if (created)
                return Ok(new { message = "Department created successfully." });

            return BadRequest("Creation failed.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] DepartmentMaster model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.DepartmentID = id;
            var updated = await _departmentService.UpdateAsync(model);
            if (updated)
                return Ok(new { message = "Department updated successfully." });

            return NotFound("Update failed. Department not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _departmentService.DeleteAsync(id);
            if (deleted)
                return Ok(new { message = "Department deleted successfully." });

            return NotFound("Delete failed. Department not found.");
        }
    }
}
