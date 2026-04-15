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
    public class RegionMasterController : ControllerBase
    {
        private readonly IRegionMasterService _regionService;

        public RegionMasterController(IRegionMasterService regionService)
        {
            _regionService = regionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRegion([FromBody] RegionMaster region)
        {
            var result = await _regionService.AddRegionAsync(region);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRegion([FromBody] RegionMaster region)
        {
            var success = await _regionService.UpdateRegionAsync(region);
            return success ? Ok("Updated") : NotFound("Region not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(decimal id)
        {
            var success = await _regionService.DeleteRegionAsync(id);
            return success ? Ok("Deleted") : NotFound("Region not found");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegionById(decimal id)
        {
            var region = await _regionService.GetRegionByIdAsync(id);
            return region != null ? Ok(region) : NotFound("Region not found");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _regionService.GetAllRegionsAsync();
            return Ok(regions);
        }
    }
}
