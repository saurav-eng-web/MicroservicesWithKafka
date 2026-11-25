using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticCnRepository : ILogisticCnRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticCnRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private static object Cell(string? value)
            => string.IsNullOrWhiteSpace(value) ? System.DBNull.Value : value.Trim();

        public async Task<int> UploadAsync(IEnumerable<CnLogisticUploadViewModel> rows, string userId)
        {
            var dt = new DataTable();
            dt.Columns.Add("CNno", typeof(string));
            dt.Columns.Add("dispatchWarehouse", typeof(string));
            dt.Columns.Add("dispatchstatus", typeof(string));
            dt.Columns.Add("docketNo", typeof(string));
            foreach (var r in rows)
                dt.Rows.Add(Cell(r.CNno), Cell(r.DispatchWarehouse), Cell(r.DispatchStatus), Cell(r.DocketNo));

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "CNLogisticUploader";
                        bulkCopy.ColumnMappings.Add("CNno", "CNno");
                        bulkCopy.ColumnMappings.Add("dispatchWarehouse", "dispatchWarehouse");
                        bulkCopy.ColumnMappings.Add("dispatchstatus", "dispatchstatus");
                        bulkCopy.ColumnMappings.Add("docketNo", "docketNo");
                        await bulkCopy.WriteToServerAsync(dt);
                    }

                    await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCn",
                        new { Type = "stampuser", UserId = userId }, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            await connection.ExecuteAsync("CNTracking", commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return dt.Rows.Count;
        }

        public async Task<IEnumerable<dynamic>> GetUploadedCnAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCn", new { Type = "get" });
        }

        public async Task<IEnumerable<dynamic>> GetChallanByTicketAsync(string ticketNo)
        {
            using var connection = _sqlConnectionService.GetConnection();
            return await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCn",
                new { Type = "challanbyticket", TicketNo = ticketNo });
        }
    }
}
