using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class LogisticUploadRepository : ILogisticUploadRepository
    {
        private readonly SQLConnectionService _sql;

        public LogisticUploadRepository(SQLConnectionService sql)
        {
            _sql = sql;
        }

        public async Task<int> BulkInsertAsync(List<LogisticUploadModel> rows)
        {
            if (rows == null || rows.Count == 0)
                return 0;

            var table = BuildTable(rows);

            using var conn = _sql.GetConnection();
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            try
            {
                using (var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction)
                {
                    DestinationTableName = "LogisticUpload",
                    BatchSize = 1000,
                    BulkCopyTimeout = 300
                })
                {
                    // Explicit mappings - IsProcess / TDate are deliberately not mapped
                    // so the table defaults (0 / getdate()) apply.
                    foreach (DataColumn column in table.Columns)
                    {
                        bulk.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    await bulk.WriteToServerAsync(table);
                }

                transaction.Commit();
                return rows.Count;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static DataTable BuildTable(List<LogisticUploadModel> rows)
        {
            var table = new DataTable();

            table.Columns.Add("TicketNumber", typeof(string));
            table.Columns.Add("SerialNumber", typeof(string));
            table.Columns.Add("CourierName", typeof(string));
            table.Columns.Add("DocketNo", typeof(string));
            table.Columns.Add("docStatus", typeof(string));
            table.Columns.Add("StatusDate", typeof(DateTime));
            table.Columns.Add("UploadedBy", typeof(string));

            foreach (var row in rows)
            {
                table.Rows.Add(
                    (object?)row.TicketNumber ?? DBNull.Value,
                    (object?)row.SerialNumber ?? DBNull.Value,
                    (object?)row.CourierName ?? DBNull.Value,
                    (object?)row.DocketNo ?? DBNull.Value,
                    (object?)row.docStatus ?? DBNull.Value,
                    (object?)row.StatusDate ?? DBNull.Value,
                    (object?)row.UploadedBy ?? DBNull.Value
                );
            }

            return table;
        }
    }
}
