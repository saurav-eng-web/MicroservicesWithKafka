using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Services.Logistic;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace MicroservicesWithKafka.Controllers.Logistic
{
    [Authorize(Policy = "LogisticPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticReportController : ControllerBase
    {
        private readonly ILogisticReportService _service;
        private readonly ILogger<LogisticReportController> _logger;

        public LogisticReportController(ILogisticReportService service, ILogger<LogisticReportController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process([FromBody] object payload)
        {
            try
            {
                var (data, code, msg) = await _service.ProcessAsync(payload);

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
                _logger.LogError(ex, "Logistic report failed");
                return StatusCode(500, LogisticResponse.Build(500, "Failed to generate the report. Please try again or contact support.", null));
            }
        }

        private IActionResult BuildExcel(LogisticReportFile file)
        {
            var rows = file.Rows;
            if (rows.Count == 0)
                return NoContent();

            var keys = rows[0].Keys.ToList();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");

            for (int c = 0; c < keys.Count; c++)
            {
                var header = sheet.Cell(1, c + 1);
                header.Value = keys[c];
                header.Style.Font.Bold = true;
            }

            sheet.Cell(2, 1).InsertData(rows.Select(r => keys.Select(k => ToCellValue(r[k])).ToArray()));
            sheet.Columns(1, keys.Count).AdjustToContents(1, Math.Min(rows.Count + 1, 200));

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                file.ReportName + ".xlsx");
        }

        private static object ToCellValue(object value)
            => value == null || value is DBNull ? string.Empty : value;
    }
}
