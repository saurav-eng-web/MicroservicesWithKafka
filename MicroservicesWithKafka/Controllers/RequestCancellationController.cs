using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]  // 🔒 Secure all endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class RequestCancellationController : ControllerBase
    {
        private readonly IRequestCancellationService _service;

        public RequestCancellationController(IRequestCancellationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(RequestCancellation model)
        {
            model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _service.CreateAsync(model);

            return Ok(response); // returns model with Message + ID
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(RequestCancellation model)
        {
            var response = await _service.UpdateAsync(model);
            return Ok(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _service.DeleteAsync(id);
            return Ok(response);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(data);
        }
    }
}
