using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class CallTypeController : ControllerBase
    {
        private readonly ICallTypeService _callTypeService;

        public CallTypeController(ICallTypeService callTypeService)
        {
            _callTypeService = callTypeService;
        }

        [HttpGet("subcategory/{subCategory}")]
        public async Task<ActionResult<List<CallTypeMaster>>> GetCallTypesBySubcategory(string subCategory)
        {
            if (string.IsNullOrWhiteSpace(subCategory))
                return BadRequest("Subcategory is required.");

            var result = await _callTypeService.GetCallTypesBySubcategoryAsync(subCategory);

            if (result == null || result.Count == 0)
                return NotFound($"No call types found for subcategory: {subCategory}");

            return Ok(result);
        }
    }
}
