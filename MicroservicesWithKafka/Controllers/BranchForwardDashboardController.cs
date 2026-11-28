using MicroservicesWithKafka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize(Roles = "BR")]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchForwardDashboardController : ControllerBase
    {
        private readonly IBranchForwardDashboardService _service;
        private readonly ILogger<BranchForwardDashboardController> _logger;

        public BranchForwardDashboardController(IBranchForwardDashboardService service, ILogger<BranchForwardDashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            try
            {
                // The branch is taken only from the signed login token
                if (!int.TryParse(User.FindFirst("Branch_ID")?.Value, out var branchId) || branchId <= 0)
                    return StatusCode(403, LogisticResponse.Build(403, "No branch is mapped to this user.", null));

                var (data, code, msg) = await _service.ProcessAsync(payload, DashboardScope.ForBranch(branchId));
                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Branch forward challan dashboard failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to load forward dashboard. Please try again or contact support.", null));
            }
        }
    }
}
