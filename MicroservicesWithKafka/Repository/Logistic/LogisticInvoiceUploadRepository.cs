using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>
    /// Translated from GST/LogisticInvoiceUploader.aspx.vb and GST/LogisticInvoiceRateUploader.aspx.vb.
    /// Legacy: SqlBulkCopy Sheet1 -> table, then "update ... set CreatedBy='<Session UName>' where IsProcess=0".
    /// (Both legacy pages then called an SP with an empty name, which did nothing, so no SP is called here.)
    /// </summary>
    public class LogisticInvoiceUploadRepository : ILogisticInvoiceUploadRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticInvoiceUploadRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        /// <summary>Blank Excel cells reached SQL as NULL through the legacy OLEDB reader.</summary>
        private static object Cell(string? value)
            => string.IsNullOrWhiteSpace(value) ? System.DBNull.Value : value.Trim();

        private async Task<int> BulkInsertAsync(DataTable dt, string table, string[] columns, string userName)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = table;
                    foreach (var c in columns)
                        bulkCopy.ColumnMappings.Add(c, c);
                    await bulkCopy.WriteToServerAsync(dt);
                }

                // A non-200 status from the SP is raised as an exception, so the rollback below still happens
                await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticInvoiceUpload",
                    new { Type = table == "LogisticInvoicePOUploader" ? "stamppo" : "stamprate", UserName = userName },
                    transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            return dt.Rows.Count;
        }

        public Task<int> UploadInvoicePoAsync(IEnumerable<LogisticInvoicePoUploadViewModel> rows, string userName)
        {
            string[] cols = { "InvoiceNo", "FromA", "Shipto", "TransporterName", "DocketNo", "VehicleType" };
            var dt = new DataTable();
            foreach (var c in cols) dt.Columns.Add(c, typeof(string));
            foreach (var r in rows)
                dt.Rows.Add(Cell(r.InvoiceNo), Cell(r.FromA), Cell(r.Shipto),
                            Cell(r.TransporterName), Cell(r.DocketNo), Cell(r.VehicleType));
            return BulkInsertAsync(dt, "LogisticInvoicePOUploader", cols, userName);
        }

        public Task<int> UploadInvoiceRateAsync(IEnumerable<LogisticInvoiceRateUploadViewModel> rows, string userName)
        {
            string[] cols = { "FromA", "ToA", "Transporter", "VehicleType", "Rate" };
            var dt = new DataTable();
            foreach (var c in cols) dt.Columns.Add(c, typeof(string));
            foreach (var r in rows)
                dt.Rows.Add(Cell(r.FromA), Cell(r.ToA), Cell(r.Transporter),
                            Cell(r.VehicleType), Cell(r.Rate));
            return BulkInsertAsync(dt, "LogisticInvoiceRateUploader", cols, userName);
        }
    }
}
