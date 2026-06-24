using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicesWithKafka.Controllers
{
    // One endpoint for ALL claim-count views -> SP_CRUD_GetFinanceClaimCounts, dispatched by $.type
    // ('financecounts' = 1 dataset, 'nsp' = 2 datasets). Plain [Authorize] (any authenticated user), legacy style.
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]   // resolves to api/FinanceClaimCount (class name minus "Controller")
    public class FinanceClaimCountController : ControllerBase
    {
        private readonly IFinanceClaimCountService _service;

        public FinanceClaimCountController(IFinanceClaimCountService service)
        {
            _service = service;
        }


        [HttpPost("claim-counts")]
        public async Task<IActionResult> ClaimCounts([FromBody] object payload)
        {
            int userId = int.TryParse(User.FindFirst("UserID")?.Value, out var u) ? u : 0;

            var (dataSets, code, msg) = await _service.GetClaimCountsAsync(payload, userId);

            // First set stays as Data/Count (single-set types like financecounts).
            // Datasets exposes EVERY set separately — nsp returns 2 (summary grid + top-10 ASC list).
            var first = dataSets != null && dataSets.Count > 0 ? dataSets[0] : new List<dynamic>();

            return StatusCode(code, new
            {
                StatusCode = code,
                StatusMsg = msg,
                Count = first.Count,
                Data = first,
                Datasets = (dataSets ?? new List<List<dynamic>>())
                    .Select(s => new { Count = s.Count, Data = s })
                    .ToList()
            });
        }
    }
}
