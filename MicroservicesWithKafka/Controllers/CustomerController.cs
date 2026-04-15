using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        // --- POST: api/customer/create ---
        [HttpPost("create")]
        public async Task<IActionResult> Create(Customer model)
        {
            var response = await _service.CreateAsync(model);

            // Use the StatusCode from the stored procedure (201 Created)
            if (response.StatusCode.HasValue)
            {
                return StatusCode(response.StatusCode.Value, response);
            }
            return CreatedAtAction(nameof(GetById), new { id = response.Cust_ID }, response);
        }

        // --- PUT: api/customer/update ---
        [HttpPut("update")]
        public async Task<IActionResult> Update(Customer model)
        {
            if (model.Cust_ID <= 0)
            {
                return BadRequest(new { StatusMsg = "Cust_ID is required for update." });
            }

            var response = await _service.UpdateAsync(model);

            // Use the StatusCode from the stored procedure (204 No Content)
            if (response.StatusCode.HasValue)
            {
                return StatusCode(response.StatusCode.Value, response);
            }
            return Ok(response);
        }

        // --- DELETE: api/customer/delete/5 ---
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);

            // Use the StatusCode from the stored procedure (204 No Content)
            if (response.StatusCode.HasValue)
            {
                return StatusCode(response.StatusCode.Value, response);
            }
            return NoContent();
        }

        //// --- GET: api/customer/all ---
        //[HttpGet("all")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var data = await _service.GetAllAsync();
        //    return Ok(data);
        //}

        // --- GET: api/customer/5 ---
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound(new { StatusMsg = $"Customer with ID {id} not found." });
            }
            return Ok(data);
        }
    }
}
