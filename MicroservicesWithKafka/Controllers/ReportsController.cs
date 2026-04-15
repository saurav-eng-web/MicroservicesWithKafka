using MicroservicesWithKafka.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ReportsController(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }
        [HttpPost("downloadreports")]
        public async Task<IActionResult> DownloadReports([FromBody] JsonElement body)
        {
            // ✅ PARAMETER VALIDATION
            if (!body.TryGetProperty("Type", out var typeEl) || string.IsNullOrWhiteSpace(typeEl.GetString()))
                return BadRequest(new { statusCode = 400, message = "Type is required." });

            if (!body.TryGetProperty("UserId", out var userEl) || string.IsNullOrWhiteSpace(userEl.GetString()))
                return BadRequest(new { statusCode = 400, message = "UserId is required." });

            string type = typeEl.GetString();
            string userId = userEl.GetString();

            DateTime? fromDate = body.TryGetProperty("FromDate", out var fd) && DateTime.TryParse(fd.GetString(), out var f)
                ? f : null;

            DateTime? toDate = body.TryGetProperty("ToDate", out var td) && DateTime.TryParse(td.GetString(), out var t)
                ? t : null;

            SqlConnection connection = (type == "RegCall1" || type == "RegCall" || type == "OpencallReport" || type == "OkCall" || type == "OkCall1" || type == "OpencallReport1") ? _sqlConnectionService.GetReportsConnection()
            : _sqlConnectionService.GetConnection();


            try
            {
                using var conn = connection;
                using var cmd = new SqlCommand("reportsNew", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@fromdate", fromDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@todate", toDate ?? (object)DBNull.Value);

                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                    return NoContent(); // 204 No data

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var sheet = workbook.Worksheets.Add("Report");

                // HEADER WITH COLOR
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var cell = sheet.Cell(1, i + 1);
                    cell.Value = reader.GetName(i);
                    cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.DarkBlue;
                    cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                    cell.Style.Font.Bold = true;
                    cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                }

                //  DATA ROWS
                int row = 2;
                while (await reader.ReadAsync())
                {
                    for (int col = 0; col < reader.FieldCount; col++)
                    {
                        var value = reader.GetValue(col);
                        var cell = sheet.Cell(row, col + 1);

                        if (value is DateTime dt)
                        {
                            cell.Value = dt; //  real date
                            cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm:ss";

                        }
                        else
                        {
                            cell.Value = value?.ToString() ?? "";
                        }
                    }
                    row++;
                }


                sheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var filename = $"{type}_Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    filename);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Database execution failed",
                    detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    statusCode = 500,
                    message = "Server error occurred",
                    detail = ex.Message
                });
            }
        }



        //private static string EscapeCSV(string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //        return "";

        //    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        //    {
        //        value = value.Replace("\"", "\"\"");
        //        return $"\"{value}\"";
        //    }

        //    return value;
        //}


    }
}
