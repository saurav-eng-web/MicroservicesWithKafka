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
    public class CityMasterController : ControllerBase
    {
        private readonly ICityMasterService _cityService;

        public CityMasterController(ICityMasterService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(decimal id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            if (city == null)
                return NotFound();

            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityMaster city)
        {
            try
            {
                var created = await _cityService.CreateCityAsync(city);
                return CreatedAtAction(nameof(GetCityById), new { id = created.CINTCODE }, created);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(decimal id, [FromBody] CityMaster city)
        {
            if (id != city.CINTCODE)
                return BadRequest("City ID mismatch.");

            var updated = await _cityService.UpdateCityAsync(city);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(decimal id)
        {
            var deleted = await _cityService.DeleteCityAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
