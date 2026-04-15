using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class ProductMasterController : ControllerBase
    {
        private readonly IProductMasterService _productService;

        public ProductMasterController(IProductMasterService productService)
        {
            _productService = productService;
        }

        // GET: api/ProductMaster
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<ProductMasterModel>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/ProductMaster
        [HttpGet("ByBrandCategory")]
        public async Task<ActionResult<IEnumerable<ProductMasterModel>>> BrandCategoryId(decimal Brandid, decimal catid)
        {
            var products = await _productService.BrandCategoryId(Brandid,catid);
            return Ok(products);
        }

        // GET: api/ProductMaster/{id}
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ProductMasterModel>> GetById(decimal id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: api/ProductMaster
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductMasterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _productService.AddAsync(model);
            return Ok(new { message = "Product added successfully" });
        }

        // POST: api/ProductMaster/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> AddMultiple([FromBody] IEnumerable<ProductMasterModel> models)
        {
            await _productService.AddMultipleAsync(models);
            return Ok(new { message = "Products added successfully" });
        }

        // DELETE: api/ProductMaster/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(decimal id)
        {
            await _productService.DeleteAsync(id);
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
