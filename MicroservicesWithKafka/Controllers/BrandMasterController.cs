using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class BrandMasterController : ControllerBase
    {
        private readonly IBrandMasterService _brandMasterService;

        public BrandMasterController(IBrandMasterService brandMasterService)
        {
            _brandMasterService = brandMasterService;
        }

        // POST: api/BrandMaster
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] BrandMasterViewModel brand)
        {
            if (brand == null)
                return BadRequest("Brand data is null.");

            var result = await _brandMasterService.CreateBrandAsync(brand);
            return Ok(result);
        }

        // PUT: api/BrandMaster
        [HttpPut]
        public async Task<IActionResult> UpdateBrand([FromBody] BrandMasterViewModel brand)
        {
            if (brand == null || brand.Brand_ID == null)
                return BadRequest("Invalid brand data.");

            var success = await _brandMasterService.UpdateBrandAsync(brand);
            return success ? Ok("Brand updated successfully.") : NotFound("Brand not found.");
        }

        // DELETE: api/BrandMaster/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(decimal id)
        {
            var success = await _brandMasterService.DeleteBrandAsync(id);
            return success ? Ok("Brand deleted successfully.") : NotFound("Brand not found.");
        }

        // GET: api/BrandMaster/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(decimal id)
        {
            var brand = await _brandMasterService.GetBrandByIdAsync(id);
            return brand != null ? Ok(brand) : NotFound("Brand not found.");
        }

        // GET: api/BrandMaster
        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandMasterService.GetAllBrandsAsync();
            return Ok(brands);
        }
    }
}
