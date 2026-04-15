using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // POST: api/Category
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryMaster category)
        {
            if (category == null)
                return BadRequest("Invalid data.");

            try
            {
                var result = await _categoryRepository.CreateCategoryAsync(category);
                return Ok(new { Message = "Category created successfully", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating category: {ex.Message}");
            }
        }

        // PUT: api/Category/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryMaster category)
        {
            if (category == null || category.Cat_ID <= 0)
                return BadRequest("Invalid category data.");

            try
            {
                bool updated = await _categoryRepository.UpdateCategoryAsync(category);
                if (updated)
                    return Ok(new { Message = "Category updated successfully" });
                else
                    return NotFound("Category not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating category: {ex.Message}");
            }
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(decimal id)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound("Category not found.");
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving category: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving categories: {ex.Message}");
            }
        }

    }
}

