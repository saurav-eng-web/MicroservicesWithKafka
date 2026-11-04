using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public class LogisticDocketRepository : ILogisticDocketRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public LogisticDocketRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private static HashSet<string> GetColumnNames(SqlDataReader reader)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
                names.Add(reader.GetName(i));
            return names;
        }

        /// <summary>Returns the first matching column value as string, or empty when the column is absent/NULL.</summary>
        private static string ReadString(SqlDataReader reader, HashSet<string> columns, params string[] candidates)
        {
            foreach (var name in candidates)
            {
                if (columns.Contains(name))
                {
                    var value = reader[name];
                    return value == DBNull.Value ? string.Empty : value.ToString() ?? string.Empty;
                }
            }
            return string.Empty;
        }

        // ------------------------------------------------------------------
        // Delete Docket (DeleteDocket.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy: DocMaster_Proc @Action='SELECT' (+ @Search = docket number when the textbox is filled).
        // The legacy grid used AutoGenerateColumns with DataKeyNames="id", so the SP column list is not
        // fixed in code. Columns are read defensively so a missing column never throws.
        public async Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search)
        {
            var dockets = new List<DocketViewModel>();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DocMaster_Proc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "SELECT");
                    // Legacy BindGrid() did not send @Search at all when the textbox was empty.
                    if (!string.IsNullOrWhiteSpace(search))
                        command.Parameters.AddWithValue("@Search", search.Trim());

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        while (await reader.ReadAsync())
                        {
                            var idText = ReadString(reader, cols, "id");
                            dockets.Add(new DocketViewModel
                            {
                                Id           = int.TryParse(idText, out var id) ? id : (int?)null,
                                ChallanNo    = ReadString(reader, cols, "ChallanNo"),
                                CourierCode  = ReadString(reader, cols, "CourierCode", "Courier", "CourierName"),
                                DocketNo     = ReadString(reader, cols, "DocketNo", "Docket"),
                                Type         = ReadString(reader, cols, "Type", "InventoryType"),
                                PickupDate   = ReadString(reader, cols, "PickupDate", "Tdate", "Date"),
                                DeliveryDate = ReadString(reader, cols, "DeliveryDate"),
                                DocStatus    = ReadString(reader, cols, "DocStatus", "IsUsed", "Status")
                            });
                        }
                    }
                }
            }

            return dockets;
        }

        // Legacy: DocMaster_Proc @Action='DELETE', @CustomerId=<grid id>. The legacy page ignored the
        // affected-row count and simply re-bound the grid, so success = the SP ran without error
        // (ExecuteNonQuery returns -1 when the SP uses SET NOCOUNT ON).
        public async Task<bool> DeleteDocketAsync(int id)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DocMaster_Proc", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "DELETE");
                    command.Parameters.AddWithValue("@CustomerId", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        // ------------------------------------------------------------------
        // History / Search History (History.aspx.vb, SearchHistory.aspx.vb)
        // ------------------------------------------------------------------

        // Legacy History.aspx.vb: sp_DocketDetails(@Type=<display name>, @PONo) -> Tables(0) header only.
        public Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo)
            => GetDocketHeaderWithHistoryAsync("sp_DocketDetails", type, challanNo);

        // Legacy SearchHistory.aspx.vb: sp_DocketDetailsSearch(@Type=<dropdown text>, @PONo) -> Tables(0).
        public Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo)
            => GetDocketHeaderWithHistoryAsync("sp_DocketDetailsSearch", type, challanNo);

        /// <summary>
        /// Reads the header row from the given SP, then (as in legacy) loads the history grid
        /// from PickUPhistory with a separate query. Returns null when the SP returns no row
        /// (legacy showed "No result found").
        /// </summary>
        private async Task<DocketDetailViewModel?> GetDocketHeaderWithHistoryAsync(string spName, string type, string challanNo)
        {
            DocketDetailViewModel? detail = null;

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand(spName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@PONo", challanNo);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var cols = GetColumnNames(reader);
                        if (await reader.ReadAsync())
                        {
                            detail = new DocketDetailViewModel
                            {
                                ChallanNo     = ReadString(reader, cols, "ChallanNo"),
                                Docket        = ReadString(reader, cols, "Docket"),
                                Courier       = ReadString(reader, cols, "Courier"),
                                TypeOfChallan = ReadString(reader, cols, "TypeOfChallan"),
                                CourierWeight = ReadString(reader, cols, "CourierWeight"),
                                Dimensions    = ReadString(reader, cols, "Dimensions"),
                                PickupBoy     = ReadString(reader, cols, "PickupBoy"),
                                PickupMobile  = ReadString(reader, cols, "PickupMobile"),
                                DocketStatus  = ReadString(reader, cols, "DocketStatus"),
                                PickupRemark  = ReadString(reader, cols, "PickupRemark")
                            };
                        }
                    }
                }

                if (detail == null)
                    return null;

                // Legacy: select PStatus,Remark,convert(nvarchar,tdate,101) As Tdate from PickUPhistory where ChallanNo='...'
                const string historyQuery =
                    "select PStatus, Remark, convert(nvarchar, tdate, 101) as Tdate from PickUPhistory where ChallanNo = @ChallanNo";

                using (var command = new SqlCommand(historyQuery, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@ChallanNo", challanNo);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            detail.History.Add(new DocketHistoryViewModel
                            {
                                PStatus = reader["PStatus"] != DBNull.Value ? reader["PStatus"].ToString() : string.Empty,
                                Remark  = reader["Remark"]  != DBNull.Value ? reader["Remark"].ToString()  : string.Empty,
                                Tdate   = reader["Tdate"]   != DBNull.Value ? reader["Tdate"].ToString()   : string.Empty
                            });
                        }
                    }
                }
            }

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

            using (var connection = _sqlConnectionService.GetConnection())
            {
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

                        using (var stamp = new SqlCommand("update DocketChange set Userid = @UserId where IsUpdate = 0", connection, transaction))
                        {
                            stamp.CommandType = CommandType.Text;
                            stamp.Parameters.AddWithValue("@UserId", userId);
                            await stamp.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                using (var command = new SqlCommand("Set_updateDocketChange", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }

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

            using (var connection = _sqlConnectionService.GetConnection())
            {
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

                using (var command = new SqlCommand("Set_updateDocketChangeStatus", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }

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
            var result = new List<DocketDownloadViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = "select (select CourierName from CourierMaster where CourierMaster.id=DocMaster.Courierid) CourierCode,(select CourierName1 from CourierMaster where CourierMaster.id=DocMaster.Courierid) Courier,DocketNo,IsUsed,InventoryType,convert(nvarchar,Tdate,101) [Date] from DocMaster";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DocketDownloadViewModel
                            {
                                CourierCode   = reader["CourierCode"]   != DBNull.Value ? reader["CourierCode"].ToString()!   : string.Empty,
                                Courier       = reader["Courier"]       != DBNull.Value ? reader["Courier"].ToString()!       : string.Empty,
                                DocketNo      = reader["DocketNo"]      != DBNull.Value ? reader["DocketNo"].ToString()!      : string.Empty,
                                IsUsed        = reader["IsUsed"]        != DBNull.Value ? reader["IsUsed"].ToString()!        : string.Empty,
                                InventoryType = reader["InventoryType"] != DBNull.Value ? reader["InventoryType"].ToString()! : string.Empty,
                                Date          = reader["Date"]          != DBNull.Value ? reader["Date"].ToString()!          : string.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        // Legacy DocketNot_Click
        public async Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync()
        {
            var result = new List<DocketNotUploadedViewModel>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();
                const string query = "select Courier as CourierCode,DocketNo,Isupload,CONVERT(nvarchar,Tdate,101) [Date] from DocMasterUpload where Isupload=0";

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DocketNotUploadedViewModel
                            {
                                CourierCode = reader["CourierCode"] != DBNull.Value ? reader["CourierCode"].ToString()! : string.Empty,
                                DocketNo    = reader["DocketNo"]    != DBNull.Value ? reader["DocketNo"].ToString()!    : string.Empty,
                                IsUpload    = reader["Isupload"]    != DBNull.Value ? reader["Isupload"].ToString()!    : string.Empty,
                                Date        = reader["Date"]        != DBNull.Value ? reader["Date"].ToString()!        : string.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        // Legacy bindgrid: Excel(CourierCode, DocketNo) -> DocMasterUpload(Courier, DocketNo) -> Set_updateDocketData
        public async Task<int> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Courier",  typeof(string));
            dt.Columns.Add("DocketNo", typeof(string));

            foreach (var row in rows)
                dt.Rows.Add(row.CourierCode?.Trim() ?? string.Empty, row.DocketNo?.Trim() ?? string.Empty);

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "DocMasterUpload";
                    bulkCopy.ColumnMappings.Add("Courier",  "Courier");
                    bulkCopy.ColumnMappings.Add("DocketNo", "DocketNo");
                    await bulkCopy.WriteToServerAsync(dt);
                }

                using (var command = new SqlCommand("Set_updateDocketData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await command.ExecuteNonQueryAsync();
                }
            }
            return dt.Rows.Count;
        }
    }
}
