using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Threading.Tasks;


namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CallCreationUploaderController : ControllerBase
    {
        private readonly ICallCreationUploaderService _service;

        public CallCreationUploaderController(ICallCreationUploaderService service)
        {
            _service = service;
        }

        //[HttpPost("create-bulk")]
        //public async Task<IActionResult> CreateBulk([FromBody] List<CallCreationUploader> models)
        //{
        //    if (models == null || models.Count == 0)
        //        return BadRequest("Invalid request: No data found");

        //    var ids = await _service.CreateBulkAsync(models);
        //    return Ok(new { Message = "Bulk Insert Successful", InsertedIds = ids });
        //}


        [HttpPost("create-bulk")]
        public async Task<IActionResult> CreateBulk(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid Excel file.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var uploads = new List<CallCreationUploader>();
            var errors = new List<object>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        // ✅ Check if row is blank (stop reading further)
                        bool isBlankRow = true;
                        for (int col = 1; col <= 13; col++) // 13 columns expected
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isBlankRow = false;
                                break;
                            }
                        }

                        if (isBlankRow)
                        {
                            break; // ⛔ stops loop completely on first empty row
                        }

                        try
                        {
                            uploads.Add(new CallCreationUploader
                            {
                                First_Name = worksheet.Cells[row, 1].Value?.ToString(),
                                Last_Name = worksheet.Cells[row, 2].Value?.ToString(),
                                Mobile = worksheet.Cells[row, 3].Value?.ToString(),
                                Alternate_Phone = worksheet.Cells[row, 4].Value?.ToString(),
                                Permanent_Address = worksheet.Cells[row, 5].Value?.ToString(),
                                Landmark = worksheet.Cells[row, 6].Value?.ToString(),

                                Pincode = long.TryParse(worksheet.Cells[row, 7].Text, out long pin) ? pin : 0,

                                Title = worksheet.Cells[row, 8].Value?.ToString(),
                                Serial_No = worksheet.Cells[row, 9].Value?.ToString(),
                                Model = worksheet.Cells[row, 10].Value?.ToString(),

                                CallBy = worksheet.Cells[row, 11].Value?.ToString() ?? "Excel Upload",
                                CallType = worksheet.Cells[row, 12].Value?.ToString(),

                                Purchase_Date = DateTime.TryParse(worksheet.Cells[row, 13].Value?.ToString(), out DateTime date) ? date : null,

                                CreatedDateTime = DateTime.Now,
                                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                            });
                        }
                        catch (Exception ex)
                        {
                            errors.Add(new
                            {
                                Row = row,
                                Error = ex.Message
                            });
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                return BadRequest(new
                {
                    Message = "Excel contains invalid data",
                    ErrorRows = errors
                });
            }

            var insertedIds = await _service.CreateBulkAsync(uploads);

            return Ok(new
            {
                Message = "Bulk Excel Uploaded Successfully",
                TotalInserted = insertedIds.Count,
                InsertedIds = insertedIds
            });
        }



        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] CallCreationUploader model)
        {
            var result = await _service.UpdateAsync(model);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return Ok(result);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return Ok(data);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }
    }
}
