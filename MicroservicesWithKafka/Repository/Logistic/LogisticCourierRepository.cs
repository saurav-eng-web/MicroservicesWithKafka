using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticCourierRepository : ILogisticCourierRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        private const int SpTimeoutSeconds = 600;

        public LogisticCourierRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<CourierViewModel>> GetCouriersAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCourier", new { Type = "get" });

            return rows.Select(r => new CourierViewModel
            {
                CourierCode = LogisticRowHelper.Str(r, "CourierName"),
                CourierName = LogisticRowHelper.Str(r, "CourierName1")
            }).ToList();
        }

        public async Task<bool> CourierExistsAsync(string courierCode)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCourier",
                new { Type = "exists", CourierCode = courierCode });
            return rows.Count > 0 && LogisticRowHelper.Int(rows[0], "Total") > 0;
        }

        public async Task<bool> AddCourierAsync(CourierViewModel courier)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCourier",
                new { Type = "insert", CourierCode = courier.CourierCode, CourierName = courier.CourierName });
            return rows.Count > 0 && LogisticRowHelper.Int(rows[0], "RowsAffected") > 0;
        }

        public async Task<IEnumerable<CourierMappingViewModel>> GetMappedCoursAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCourier", new { Type = "mapped" },
                commandTimeout: SpTimeoutSeconds);

            return rows.Select(r => new CourierMappingViewModel
            {
                CourierCode = LogisticRowHelper.Str(r, "CourierCode"),
                CourierName = LogisticRowHelper.Str(r, "CourierName"),
                Pincode     = LogisticRowHelper.Str(r, "Pincode"),
                Slab        = LogisticRowHelper.Str(r, "Slab"),
                Priority    = LogisticRowHelper.Str(r, "Priority"),
                Date        = LogisticRowHelper.Str(r, "Date")
            }).ToList();
        }

        public async Task<IEnumerable<CourierMappingViewModel>> GetUnmappedCoursAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticCourier", new { Type = "unmapped" },
                commandTimeout: SpTimeoutSeconds);

            return rows.Select(r => new CourierMappingViewModel
            {
                CourierCode = LogisticRowHelper.Str(r, "Couriercode"),
                Pincode     = LogisticRowHelper.Str(r, "Pincode"),
                Slab        = LogisticRowHelper.Str(r, "Slab"),
                Priority    = LogisticRowHelper.Str(r, "Priority"),
                Action      = LogisticRowHelper.Str(r, "Action"),
                Date        = LogisticRowHelper.Str(r, "Date")
            }).ToList();
        }

        public async Task<bool> UploadCourierMappingAsync(IEnumerable<CourierMappingUploadViewModel> rows)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            var dt = new DataTable();
            dt.Columns.Add("Courier",  typeof(string));
            dt.Columns.Add("Pincode",  typeof(string));
            dt.Columns.Add("Slab",     typeof(string));
            dt.Columns.Add("Priority", typeof(string));
            dt.Columns.Add("Action",   typeof(string));

            foreach (var row in rows)
                dt.Rows.Add(row.CourierCode, row.Pincode, row.Slab, row.Priority, row.Action);

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "CourMappingUpload";
                bulkCopy.ColumnMappings.Add("Courier",  "Courier");
                bulkCopy.ColumnMappings.Add("Pincode",  "Pincode");
                bulkCopy.ColumnMappings.Add("Slab",     "Slab");
                bulkCopy.ColumnMappings.Add("Priority", "Priority");
                bulkCopy.ColumnMappings.Add("Action",   "Action");
                await bulkCopy.WriteToServerAsync(dt);
            }

            await connection.ExecuteAsync("Set_CourierMapping",
                commandType: CommandType.StoredProcedure, commandTimeout: SpTimeoutSeconds);

            return true;
        }
    }
}
