using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using OfficeOpenXml;

namespace MicroservicesWithKafka.Services
{
    public class LogisticUploadService : ILogisticUploadService
    {
        private const int ExpectedColumns = 6;

        private readonly ILogisticUploadRepository _repo;

        public LogisticUploadService(ILogisticUploadRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, int, string)>
            ProcessAsync(IFormFile file, string uploadedBy)
        {
            if (file == null || file.Length == 0)
                return (Enumerable.Empty<dynamic>(), 0, 400, "Please upload a valid Excel file.");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var uploads = new List<LogisticUploadModel>();
            var errors = new List<dynamic>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet?.Dimension == null)
                        return (Enumerable.Empty<dynamic>(), 0, 400, "Excel file has no data.");

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Check if row is blank (stop reading further)
                        bool isBlankRow = true;
                        for (int col = 1; col <= ExpectedColumns; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isBlankRow = false;
                                break;
                            }
                        }

                        if (isBlankRow)
                        {
                            break; // stops loop completely on first empty row
                        }

                        try
                        {
                            string ticketNumber = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            string serialNumber = worksheet.Cells[row, 2].Value?.ToString()?.Trim();

                            // The SP promotes rows by matching TicketNo + SerialNumber,
                            // so a row missing either can never be processed.
                            if (string.IsNullOrWhiteSpace(ticketNumber))
                            {
                                errors.Add(new { Row = row, Error = "TicketNumber is required." });
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(serialNumber))
                            {
                                errors.Add(new { Row = row, Error = "SerialNumber is required." });
                                continue;
                            }

                            uploads.Add(new LogisticUploadModel
                            {
                                TicketNumber = ticketNumber,
                                SerialNumber = serialNumber,
                                CourierName = worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                                DocketNo = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                                docStatus = worksheet.Cells[row, 5].Value?.ToString()?.Trim(),
                                StatusDate = ReadDate(worksheet.Cells[row, 6].Value),
                                UploadedBy = uploadedBy
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

            // All-or-nothing: nothing is inserted when any row failed
            if (errors.Count > 0)
                return (errors, 0, 400, "Excel contains invalid data");

            if (uploads.Count == 0)
                return (Enumerable.Empty<dynamic>(), 0, 400, "Excel file has no data rows.");

            int inserted = await _repo.BulkInsertAsync(uploads);

            return (Enumerable.Empty<dynamic>(), inserted, 200, "Uploaded Successfully");
        }

        /// <summary>
        /// Excel hands dates back as DateTime, as an OLE Automation serial number,
        /// or as plain text depending on how the cell was formatted.
        /// </summary>
        private static DateTime? ReadDate(object value)
        {
            if (value == null)
                return null;

            if (value is DateTime dateTime)
                return dateTime;

            if (value is double serial)
                return DateTime.FromOADate(serial);

            return DateTime.TryParse(value.ToString(), out var parsed) ? parsed : null;
        }
    }
}
