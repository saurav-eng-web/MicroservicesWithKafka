using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize] // 🔒 Applies to all actions below
    [ApiController]
    [Route("api/[controller]")]
    public class WarrantyRegistrationController : ControllerBase
    {
        private readonly IWarrantyRegistrationService _service;

        public WarrantyRegistrationController(IWarrantyRegistrationService service)
        {
            _service = service;
        }

        // POST: api/WarrantyRegistration

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] WarrantyRegistration model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var id = await _service.AddAsync(model);
        //    return Ok(new { Message = "Created", Id = id });
        //}


        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] WarrantyRegistration model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _service.AddAsync(model);

            return Ok(new { Message = "Created", Id = id });
        }


        [HttpPost("upload-invoice")]
        public async Task<IActionResult> UploadInvoice(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/invoices");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return only the file name or path to the client
            return Ok(new { FileName = uniqueFileName });
        }

        // PUT: api/WarrantyRegistration/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] WarrantyRegistration model)
        {
            if (!ModelState.IsValid || id != model.ID)
                return BadRequest("Invalid data or mismatched ID.");

            var updated = await _service.UpdateAsync(model);
            if (!updated)
                return NotFound("Warranty record not found");

            return Ok(new { Message = "Updated" });
        }

        // DELETE: api/WarrantyRegistration/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound("Warranty record not found");

            return Ok(new { Message = "Deleted" });
        }

        // GET: api/WarrantyRegistration
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _service.GetAllAsync();
            return Ok(records);
        }

        // GET: api/WarrantyRegistration/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var record = await _service.GetByIdAsync(id);
            if (record == null)
                return NotFound("Warranty record not found");

            return Ok(record);
        }

        // GET: api/WarrantyRegistration/by-serial/{serialNumber}
        [HttpGet("by-serial/{serialNumber}")]
        public async Task<IActionResult> GetBySerialNumber(string serialNumber)
        {
            var record = await _service.GetBySerialNumberAsync(serialNumber);
            if (record == null)
                return NotFound("No warranty found for this serial number");

            return Ok(record);
        }
    }
}
