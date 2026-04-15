using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobServiceService _service;

        public JobController(IJobServiceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] JobService job)
        {
            var createdJob = await _service.CreateJobAsync(job);
            return createdJob != null ? Ok(createdJob) : BadRequest("Failed to create job");
        }
    }

}
