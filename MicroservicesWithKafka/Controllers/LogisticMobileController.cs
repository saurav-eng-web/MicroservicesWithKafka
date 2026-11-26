using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticMobileController : ControllerBase
    {
        private readonly ILogisticMobileService _service;
        private readonly ILogger<LogisticMobileController> _logger;

        public LogisticMobileController(ILogisticMobileService service, ILogger<LogisticMobileController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string CurrentUserId() => User.FindFirst("UserID")?.Value ?? string.Empty;

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
            if (string.IsNullOrWhiteSpace(ip))
                ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload, CurrentUserId(), ip);

                if (code == 200 && data is LogisticReportFile file)
                    return BuildExcel(file);

                return StatusCode(code, LogisticResponse.Build(code, msg, data));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(LogisticResponse.Build(400, ex.Message, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistic mobile process failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to process mobile request. Please try again or contact support.", null));
            }
        }

        private IActionResult BuildExcel(LogisticReportFile file)
        {
            var rows = file.Rows;
            if (rows.Count == 0)
                return NoContent();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Sheet1");
            var columns = rows[0].Keys.ToList();
            for (int c = 0; c < columns.Count; c++)
            {
                var cell = sheet.Cell(1, c + 1);
                cell.Value = columns[c];
                cell.Style.Font.Bold = true;
            }
            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < columns.Count; c++)
                {
                    var value = rows[r][columns[c]];
                    var cell = sheet.Cell(r + 2, c + 1);
                    if (value is DateTime dt)
                    {
                        cell.Value = dt;
                        cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm:ss";
                    }
                    else
                    {
                        cell.Value = value?.ToString() ?? string.Empty;
                    }
                }
            }
            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{file.ReportName}.xlsx");
        }
    }
}
