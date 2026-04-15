using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly IModelService _modelService;

        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }

        // POST: api/Model
        [HttpPost]
        public async Task<IActionResult> PostModel([FromBody] ModelMasterViewModel model)
        {
            try
            {
                var createdModel = await _modelService.CreateModelAsync(model);
                return Ok(createdModel);
            }
            catch (Exception ex)
            {
                // Log ex.Message or ex.ToString() here for debugging
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/Model
        [HttpPut]
        public async Task<IActionResult> UpdateModel([FromBody] ModelMasterViewModel model)
        {
            if (model?.ModelID == null)
                return BadRequest("Model ID is required for update.");

            try
            {
                var result = await _modelService.UpdateModelAsync(model);
                if (!result)
                    return NotFound("Model not found.");

                return Ok("Model updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // DELETE: api/Model/{modelId}
        [HttpDelete("{modelId}")]
        public async Task<IActionResult> DeleteModel(decimal modelId)
        {
            try
            {
                var result = await _modelService.DeleteModelAsync(modelId);
                if (!result)
                    return NotFound("Model not found.");

                return Ok("Model deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // GET: api/Model/{modelId}
        [HttpGet("id/{modelId}")]
        public async Task<IActionResult> GetModelById(decimal modelId)
        {
            try
            {
                var model = await _modelService.GetModelByIdAsync(modelId);
                if (model == null)
                    return NotFound("Model not found.");

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // GET: api/Model/{ProductId}
        [HttpGet("productid/{productId}")]
        public async Task<IActionResult> GetModelsByProductId(decimal productId)
        {
            try
            {
                var models = await _modelService.GetModelsByProductIdAsync(productId);
                if (models == null || models.Count == 0)
                    return NotFound("No models found for this product.");

                return Ok(models);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        // GET: api/Model
        [HttpGet]
        public async Task<IActionResult> GetAllModels()
        {
            try
            {
                var models = await _modelService.GetAllModelsAsync();
                return Ok(models);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }
    }
}
