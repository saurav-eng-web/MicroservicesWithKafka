using Dapper;
using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticDocketRepository : ILogisticDocketRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        // Legacy SqlProceedure used CommandTimeout = 2000000 (effectively no limit). The upload SPs process the whole staging table and can
        // take longer than the 30 s ADO.NET default, which made the upload fail with HTTP 500.
        private const int SpTimeoutSeconds = 600;

        public LogisticDocketRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private static string S(IDictionary<string, object?> row, params string[] names) => LogisticRowHelper.Str(row, names);

        // ------------------------------------------------------------------
        // Delete Docket (DeleteDocket.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy: DocMaster_Proc @Action='SELECT' (+ @Search = docket number when the textbox is filled).
        // The legacy grid used AutoGenerateColumns with DataKeyNames="id", so the SP column list is not
        // fixed in code. Columns are read defensively so a missing column never throws.
        public async Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search)
        {
            // Legacy BindGrid() did not send @Search at all when the textbox was empty.
            var parameters = new DynamicParameters();
            parameters.Add("Action", "SELECT");
            if (!string.IsNullOrWhiteSpace(search))
                parameters.Add("Search", search.Trim());

            using var connection = _sqlConnectionService.GetConnection();
            var rows = await connection.QueryAsync("DocMaster_Proc", parameters, commandType: CommandType.StoredProcedure);

            return LogisticRowHelper.ToRows(rows).Select(r => new DocketViewModel
            {
                Id           = int.TryParse(S(r, "id"), out var id) ? id : (int?)null,
                ChallanNo    = S(r, "ChallanNo"),
                // DocMaster_Proc SELECT returns exactly [ID], [Courier Name], [Docket No]
                // ("Courier Name" is CourierMaster.CourierName, i.e. the courier code).
                CourierCode  = S(r, "Courier Name", "CourierCode", "Courier", "CourierName"),
                DocketNo     = S(r, "Docket No", "DocketNo", "Docket"),
                Type         = S(r, "Type", "InventoryType"),
                PickupDate   = S(r, "PickupDate", "Tdate", "Date"),
                DeliveryDate = S(r, "DeliveryDate"),
                DocStatus    = S(r, "DocStatus", "IsUsed", "Status")
            }).ToList();
        }

        // Legacy: DocMaster_Proc @Action='DELETE', @CustomerId=<grid id>. The legacy page ignored the
        // affected-row count and simply re-bound the grid, so success = the SP ran without error.
        public async Task<bool> DeleteDocketAsync(int id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.ExecuteAsync("DocMaster_Proc", new { Action = "DELETE", CustomerId = id },
                commandType: CommandType.StoredProcedure);
            return true;
        }

        // ------------------------------------------------------------------
        // History / Search History (History.aspx.vb, SearchHistory.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy History.aspx.vb: sp_DocketDetails(@Type=<display name>, @PONo) -> Tables(0) header only.
        // sp_DocketDetails only returns challans whose DocketStatus is 'Dispatched', so In-Transit / Pickup Awaited challans
        // (now listed by Status View) came back with an empty header. sp_DocketDetailsSearch has the same columns for every
        // docket status, so the history header uses it.
        public Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo)
            => GetDocketHeaderWithHistoryAsync("sp_DocketDetailsSearch", type, challanNo, allowEmptyHeader: true);

        // Legacy SearchHistory.aspx.vb: sp_DocketDetailsSearch(@Type=<dropdown text>, @PONo) -> Tables(0).
        public Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo)
            => GetDocketHeaderWithHistoryAsync("sp_DocketDetailsSearch", type, challanNo, allowEmptyHeader: false);

        /// <summary>
        /// Reads the header row from the given SP, then (as in legacy) loads the history grid
        /// from PickUPhistory (SP_CRUD_LogisticDocket HISTORY). Returns null when the SP returns no row
        /// (legacy showed "No result found").
        /// </summary>
        /// <param name="allowEmptyHeader">
        /// History.aspx always loaded the PickUPhistory grid even when sp_DocketDetails returned no header row,
        /// so History passes true (null is returned only when there is no header AND no history rows).
        /// SearchHistory.aspx exited early with "No result found" when the SP returned no row, so Search passes false.
        /// </param>
        private async Task<DocketDetailViewModel?> GetDocketHeaderWithHistoryAsync(string spName, string type, string challanNo, bool allowEmptyHeader)
        {
            using var connection = _sqlConnectionService.GetConnection();

            var headerRows = await connection.QueryAsync(spName, new { Type = type, PONo = challanNo },
                commandType: CommandType.StoredProcedure);
            var header = LogisticRowHelper.ToRows(headerRows).FirstOrDefault();

            bool headerFound = header != null;
            if (!headerFound && !allowEmptyHeader)
                return null;

            var detail = headerFound
                ? new DocketDetailViewModel
                {
                    ChallanNo     = S(header!, "ChallanNo"),
                    Docket        = S(header!, "Docket"),
                    Courier       = S(header!, "Courier"),
                    TypeOfChallan = S(header!, "TypeOfChallan"),
                    CourierWeight = S(header!, "CourierWeight"),
                    Dimensions    = S(header!, "Dimensions"),
                    PickupBoy     = S(header!, "PickupBoy"),
                    PickupMobile  = S(header!, "PickupMobile"),
                    DocketStatus  = S(header!, "DocketStatus"),
                    PickupRemark  = S(header!, "PickupRemark")
                }
                : new DocketDetailViewModel { ChallanNo = challanNo };

            // Legacy: select PStatus,Remark,convert(nvarchar,tdate,101) As Tdate from PickUPhistory where ChallanNo='...'
            var historyRows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDocket",
                new { Type = "history", ChallanNo = challanNo });
            foreach (var h in historyRows)
            {
                detail.History.Add(new DocketHistoryViewModel
                {
                    PStatus = S(h, "PStatus"),
                    Remark  = S(h, "Remark"),
                    Tdate   = S(h, "Tdate")
                });
            }

            // No header and no history rows: nothing to show ("No result found").
            if (!headerFound && detail.History.Count == 0)
                return null;

            return detail;
        }

        // ------------------------------------------------------------------
        // Update Docket Details (DocketChange.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy: Excel(ChallanNo, CourierCode, DocketNo, Type) -> SqlBulkCopy into DocketChange with
        // mappings ChallanNo->ChallanNo, CourierCode->CourierCode, DocketNo->DocketNo, Type->Type;
        // then "Update DocketChange set Userid='<uidfic>' where IsUpdate=0"; then Set_updateDocketChange.
        public async Task<int> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId)
        {
            var dt = new DataTable();
            dt.Columns.Add("ChallanNo",   typeof(string));
            dt.Columns.Add("CourierCode", typeof(string));
            dt.Columns.Add("DocketNo",    typeof(string));
            dt.Columns.Add("Type",        typeof(string));

            foreach (var row in rows)
            {
                dt.Rows.Add(
                    row.ChallanNo?.Trim()   ?? string.Empty,
                    row.CourierCode?.Trim() ?? string.Empty,
                    row.DocketNo?.Trim()    ?? string.Empty,
                    row.Type?.Trim()        ?? string.Empty);
            }

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            // Bulk copy + user stamp are atomic; the SP runs afterwards exactly as in legacy.
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "DocketChange";
                        bulkCopy.ColumnMappings.Add("ChallanNo",   "ChallanNo");
                        bulkCopy.ColumnMappings.Add("CourierCode", "CourierCode");
                        bulkCopy.ColumnMappings.Add("DocketNo",    "DocketNo");
                        bulkCopy.ColumnMappings.Add("Type",        "Type");
                        await bulkCopy.WriteToServerAsync(dt);
                    }

                    // A non-200 status from the SP is raised as an exception, so the rollback below still happens
                    await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDocket",
                        new { Type = "stampuser", UserId = userId }, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            await connection.ExecuteAsync("Set_updateDocketChange",
                commandType: CommandType.StoredProcedure, commandTimeout: SpTimeoutSeconds);

            return dt.Rows.Count;
        }

        // ------------------------------------------------------------------
        // Update Docket Status (DocketChangeStatus.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy mappings: ChallanNo->ChallanNo, Type->Type, PickupDate->PickupDate,
        // DeliveryDate->DeliveryDate, Status->DocStatus; then Set_updateDocketChangeStatus.
        // Dates are sent as DateTime values, the same as the legacy OLEDB Excel reader produced.
        public async Task<int> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("ChallanNo",    typeof(string));
            dt.Columns.Add("Type",         typeof(string));
            dt.Columns.Add("PickupDate",   typeof(DateTime));
            dt.Columns.Add("DeliveryDate", typeof(DateTime));
            dt.Columns.Add("DocStatus",    typeof(string));

            foreach (var row in rows)
            {
                dt.Rows.Add(
                    row.ChallanNo?.Trim() ?? string.Empty,
                    row.Type?.Trim()      ?? string.Empty,
                    ParseDate(row.PickupDate),
                    ParseDate(row.DeliveryDate),
                    row.DocStatus?.Trim() ?? string.Empty);
            }

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "DocketChangeStatus";
                bulkCopy.ColumnMappings.Add("ChallanNo",    "ChallanNo");
                bulkCopy.ColumnMappings.Add("Type",         "Type");
                bulkCopy.ColumnMappings.Add("PickupDate",   "PickupDate");
                bulkCopy.ColumnMappings.Add("DeliveryDate", "DeliveryDate");
                bulkCopy.ColumnMappings.Add("DocStatus",    "DocStatus");
                await bulkCopy.WriteToServerAsync(dt);
            }

            await connection.ExecuteAsync("Set_updateDocketChangeStatus",
                commandType: CommandType.StoredProcedure, commandTimeout: SpTimeoutSeconds);

            return dt.Rows.Count;
        }

        /// <summary>
        /// Converts an uploaded date string to a DateTime (or DBNull when blank).
        /// Accepts ISO dates, dd-MM-yyyy / dd/MM/yyyy and raw Excel serial numbers (e.g. 45267),
        /// because real legacy files store Excel dates. Throws FormatException for anything else.
        /// </summary>
        public static object ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            var text = value.Trim();
            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (double.TryParse(text, System.Globalization.NumberStyles.Float, culture, out var serial)
                && serial > 0 && serial < 2958466)
                return DateTime.FromOADate(serial).Date;

            string[] formats =
            {
                "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ssZ",
                "dd-MM-yyyy", "dd/MM/yyyy", "d-M-yyyy", "d/M/yyyy", "dd-MMM-yyyy", "dd MMM yyyy"
            };
            if (DateTime.TryParseExact(text, formats, culture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out var exact))
                return exact.Date;

            throw new FormatException($"Invalid date '{value}'. Use yyyy-MM-dd or dd-MM-yyyy.");
        }

        // ------------------------------------------------------------------
        // Docket Upload (root DocketUpload.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy DownloadDocket_Click
        public async Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDocket", new { Type = "download" },
                commandTimeout: SpTimeoutSeconds);

            return rows.Select(r => new DocketDownloadViewModel
            {
                CourierCode   = S(r, "CourierCode"),
                Courier       = S(r, "Courier"),
                DocketNo      = S(r, "DocketNo"),
                IsUsed        = S(r, "IsUsed"),
                InventoryType = S(r, "InventoryType"),
                Date          = S(r, "Date")
            }).ToList();
        }

        // Legacy DocketNot_Click
        public async Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var rows = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticDocket", new { Type = "notuploaded" },
                commandTimeout: SpTimeoutSeconds);

            return rows.Select(r => new DocketNotUploadedViewModel
            {
                CourierCode = S(r, "CourierCode"),
                DocketNo    = S(r, "DocketNo"),
                IsUpload    = S(r, "Isupload"),
                Date        = S(r, "Date")
            }).ToList();
        }

        // ------------------------------------------------------------------
        // Forward Logistic Docket Uploader
        // ------------------------------------------------------------------

        // The Excel rows go into challdocketstusuploder (IsProcess = 0, CreatedBy = login user name) and SP_CRUD_LogisticForwardDocket then updates
        // tbl_PORecieveHistory (In-Transit -> Docket_transit_Date, Delivered -> Docket_Delivery_date). Everything runs in ONE transaction:
        // if the SP fails (e.g. a future Trans_Date) the staged rows are rolled back too, so nothing is left half done.
        public async Task<ForwardDocketUploadResult> UploadForwardDocketsAsync(IEnumerable<ForwardDocketRow> rows, string userName)
        {
            var dt = new DataTable();
            dt.Columns.Add("Challan_No",   typeof(string));
            dt.Columns.Add("Docket_No",    typeof(string));
            dt.Columns.Add("DOC_status",   typeof(string));
            dt.Columns.Add("Trans_Date",   typeof(DateTime));
            dt.Columns.Add("Upload_Type",  typeof(string));
            dt.Columns.Add("Challan_Type", typeof(string));
            dt.Columns.Add("CreatedBy",    typeof(string));

            foreach (var row in rows)
                dt.Rows.Add(row.ChallanNo, row.DocketNo, row.DocStatus, row.TransDate, row.UploadType, row.ChallanType, userName);

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();
            try
            {
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "challdocketstusuploder";
                    bulkCopy.BulkCopyTimeout = SpTimeoutSeconds;
                    foreach (DataColumn column in dt.Columns)
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName); // IsProcess (0) and TDate (getdate()) use the table defaults
                    await bulkCopy.WriteToServerAsync(dt);
                }

                var notApplied = await LogisticSpRunner.RunAsync(connection, "SP_CRUD_LogisticForwardDocket",
                    new { Type = "process", UserId = userName }, transaction, SpTimeoutSeconds);

                transaction.Commit();

                return new ForwardDocketUploadResult
                {
                    Uploaded = dt.Rows.Count,
                    NotApplied = notApplied.Select(r => new ForwardDocketNotAppliedViewModel
                    {
                        ChallanNo = S(r, "ChallanNo"),
                        DocketNo  = S(r, "DocketNo"),
                        DocStatus = S(r, "DocStatus"),
                        Reason    = S(r, "Reason")
                    }).ToList()
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Legacy bindgrid: Excel(CourierCode, DocketNo) -> DocMasterUpload(Courier, DocketNo) -> Set_updateDocketData
        public async Task<int> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Courier",  typeof(string));
            dt.Columns.Add("DocketNo", typeof(string));

            foreach (var row in rows)
                dt.Rows.Add(row.CourierCode?.Trim() ?? string.Empty, row.DocketNo?.Trim() ?? string.Empty);

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "DocMasterUpload";
                bulkCopy.ColumnMappings.Add("Courier",  "Courier");
                bulkCopy.ColumnMappings.Add("DocketNo", "DocketNo");
                await bulkCopy.WriteToServerAsync(dt);
            }

            await connection.ExecuteAsync("Set_updateDocketData",
                commandType: CommandType.StoredProcedure, commandTimeout: SpTimeoutSeconds);

            return dt.Rows.Count;
        }
    }
}
