using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    // hide swagger
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentUploadController : ControllerBase
    {
        private readonly IDocumentUploadService _service;
        private readonly NasSettings _nas;

        public DocumentUploadController(IDocumentUploadService service, IOptions<NasSettings> nasSettings)
        {
            _service = service;
            _nas = nasSettings.Value;
        }

        // ✅ UPDATED: Upload file to NAS + Save URL + Save metadata
        //[Authorize(Policy = "AdminPolicy")] // 👈 Restrict to Admin role
        [HttpPost("upload")]
        public async Task<IActionResult> InsertUpload([FromForm] IFormFile file, [FromForm] string ticketno, [FromForm] string documentType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            if (string.IsNullOrWhiteSpace(ticketno))
                return BadRequest("TicketKey is required");

            // Create a unique filename for the NAS
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(_nas.RootPath, fileName);

            //Upload file to NAS (Windows: N:\ or Linux: /mnt/nas/)
            Directory.CreateDirectory(_nas.RootPath); // Ensure path exists
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 🌐 File URL to save in DB
            string fileUrl = $"{_nas.BaseUrl}/{fileName}".Replace("\\", "/");

            var doc = new DocumentUpload
            {
                Ticket_no = ticketno,
                DocumentURL = fileUrl,
                DocumentType = documentType,
                Status = "A"
            };

            var id = await _service.InsertUploadAsync(doc);

            return Ok(new { NewID = id, Url = fileUrl });
        }

        // ✅ INSERT PART DETAILS ONLY (without file)
        [HttpPost("part")]
        public async Task<IActionResult> InsertPart([FromBody] DocumentUpload doc)
        {
            var id = await _service.InsertPartAsync(doc);
            return Ok(new { NewID = id });
        }

        // ✅ UPDATE DOCUMENT ENTRY
        [HttpPut("upload/{id}")]
        public async Task<IActionResult> UpdateUpload(int id, [FromBody] DocumentUpload doc)
        {
            doc.DocumentID = id;
            await _service.UpdateUploadAsync(doc);
            return NoContent();
        }

        // ✅ UPDATE PART ENTRY
        [HttpPut("part/{id}")]
        public async Task<IActionResult> UpdatePart(int id, [FromBody] DocumentUpload doc)
        {
            doc.DocumentID = id;
            await _service.UpdatePartAsync(doc);
            return NoContent();
        }

        // ✅ DELETE UPLOADED DOCUMENT ENTRY
        [HttpDelete("upload/{id}")]
        public async Task<IActionResult> DeleteUpload(int id)
        {
            await _service.DeleteUploadAsync(id);
            return NoContent();
        }

        // ✅ DELETE PART ENTRY
        [HttpDelete("part/{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            await _service.DeletePartAsync(id);
            return NoContent();
        }

        // ✅ GET DOCUMENTS BY TICKET
        [HttpGet("ticket/{ticketKey}")]
        public async Task<IActionResult> GetByTicket(string ticketKey)
        {
            string accessUser = User.FindFirst("username")?.Value ?? "0";
            var result = await _service.GetByTicketAsync(ticketKey,accessUser);
            return Ok(result);
        }
    }
}
