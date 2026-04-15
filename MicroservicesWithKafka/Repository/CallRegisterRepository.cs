


using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CallRegisterRepository : ICallRegisterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CallRegisterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }


        public async Task<string?> CreateComplaintAsync(CallRegisterModel model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("Complaint_Save_NewCRM", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Map all SP parameters from NewCallRegisterModel
            command.Parameters.AddWithValue("@SALUTATION", model.SALUTATION ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FIRSTNAME", model.FIRSTNAME ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LASTNAME", model.LASTNAME ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@HMPH", model.HMPH ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@WKPH", model.WKPH ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@stdcode", model.STDCODE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@pincode", model.PINCODE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MOBILE", model.MOBILE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ADDRESS1", model.ADDRESS1 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ADDRESS2", model.ADDRESS2 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@City_ID", model.CITY_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Locality_ID", model.LOCALITY_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@landmark", model.LANDMARK ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Call_Category", model.CALL_CATEGORY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Call_TypeID", model.CALL_TYPEID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CallBy_ID", model.CALLBY_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@WarrantyID", model.WARRANTYID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CALL_TYPE", model.CALL_TYPE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CALL_BY", model.CALL_BY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Dealer", model.DEALER ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@WARRANTY", model.WARRANTY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PRODUCT", model.PRODUCT ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Product_ID", model.PRODUCT_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model_ID", model.MODEL_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PURCHASE_DATE", model.PURCHASE_DATE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SERIAL_NO", model.SERIAL_NO ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint", model.COMPLAINT ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Sf_ID", model.SF_ID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PROMISE_DATE", model.PROMISE_DATE ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PROMISE_TIME", model.PROMISE_TIME ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Category", model.CATEGORY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnterBy", model.ENTERBY ?? (object)DBNull.Value);
            //command.Parameters.AddWithValue("@COMPLAINT_STATUS", model.COMPLAINT_STATUS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", model.REMARKS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", model.EMAIL ?? (object)DBNull.Value);
            //command.Parameters.AddWithValue("@Complaint_SubStatus", model.COMPLAINT_SUBSTATUS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Excalated", model.EXCALATED ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Legal", model.LEGAL ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredBy", model.ENTEREDBY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredByID", model.ENTEREDBYID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IPAddress", model.IPADDRESS ?? (object)DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Assuming stored procedure returns: SELECT @ReturnNo AS ReturnNo
                return reader["ReturnNo"]?.ToString();
            }

            return null;
        }


        public async Task<CallRegisterModel?> GetComplaintByTicketNoAsync(string ticketNo)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_Get_ByTicketNo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@TicketNo", ticketNo);
            command.Parameters.AddWithValue("@type", "Ticketno");

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var model = new CallRegisterModel();
                var props = typeof(CallRegisterModel).GetProperties();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var prop = props.FirstOrDefault(p =>
                        string.Equals(p.Name, columnName, StringComparison.OrdinalIgnoreCase));

                    if (prop != null && !await reader.IsDBNullAsync(i))
                    {
                        var value = reader.GetValue(i);

                        // Handle empty strings for numeric/date fields
                        if (value is string strVal && string.IsNullOrWhiteSpace(strVal))
                        {
                            continue; // leave property as null/default
                        }

                        try
                        {
                            if (prop.PropertyType.IsGenericType &&
                                prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                prop.SetValue(model, Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType)));
                            }
                            else
                            {
                                prop.SetValue(model, Convert.ChangeType(value, prop.PropertyType));
                            }
                        }
                        catch
                        {
                            // Skip invalid conversions instead of throwing
                            continue;
                        }
                    }
                }

                return model;
            }

            return null;
        }

        public async Task<List<CallRegisterModel>> GetComplaintsByStatusAsync(string? complaintStatus, string? complaintSubStatus,int? role,int? bid)
        {
            var complaints = new List<CallRegisterModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_Get_ByTicketNo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@cst", (object?)complaintStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@csst", (object?)complaintSubStatus ?? DBNull.Value);
            command.Parameters.AddWithValue("@role", (object?)role ?? DBNull.Value);
            command.Parameters.AddWithValue("@bid", (object?)bid ?? DBNull.Value);
            command.Parameters.AddWithValue("@type", "Status");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                complaints.Add(new CallRegisterModel
                {
                    CallId = reader["CallId"] as int?,
                    SALUTATION = reader["SALUTATION"]?.ToString(),
                    FIRSTNAME = reader["FIRSTNAME"]?.ToString(),
                    LASTNAME = reader["LASTNAME"]?.ToString(),
                    HMPH = reader["HMPH"]?.ToString(),
                    WKPH = reader["WKPH"]?.ToString(),
                    STDCODE = reader["STDCODE"]?.ToString(),
                    PINCODE = reader["PINCODE"]?.ToString(),
                    MOBILE = reader["MOBILE"]?.ToString(),
                    ADDRESS1 = reader["ADDRESS1"]?.ToString(),
                    ADDRESS2 = reader["ADDRESS2"]?.ToString(),
                    CITY_ID = reader["CITY_ID"] != DBNull.Value ? Convert.ToInt32(reader["CITY_ID"]) : (int?)null,
                    TICKET_NO = reader["TICKET_NO"]?.ToString(),
                    LOCALITY_ID = reader["LOCALITY_ID"] != DBNull.Value ? Convert.ToInt32(reader["LOCALITY_ID"]) : (int?)null,
                    LANDMARK = reader["LANDMARK"]?.ToString(),
                    CALL_CATEGORY = reader["CALL_CATEGORY"]?.ToString(),
                    CALL_TYPEID = reader["CALL_TYPEID"] != DBNull.Value ? Convert.ToInt32(reader["CALL_TYPEID"]) : (int?)null,
                    CALLBY_ID = reader["CALLBY_ID"] != DBNull.Value ? Convert.ToInt32(reader["CALLBY_ID"]) : (int?)null,
                    WARRANTYID = reader["WARRANTYID"] != DBNull.Value ? Convert.ToInt32(reader["WARRANTYID"]) : (int?)null,

                    CALL_TYPE = reader["CALL_TYPE"]?.ToString(),
                    CALL_BY = reader["CALL_BY"]?.ToString(),
                    DEALER = reader["DEALER"]?.ToString(),
                    WARRANTY = reader["WARRANTY"]?.ToString(),
                    PRODUCT = reader["PRODUCT"]?.ToString(),
                    PRODUCT_ID = reader["PRODUCT_ID"] != DBNull.Value ? Convert.ToInt32(reader["PRODUCT_ID"]) : (int?)null,
                    MODEL_ID = reader["MODEL_ID"] != DBNull.Value ? Convert.ToInt32(reader["MODEL_ID"]) : (int?)null,
                    PURCHASE_DATE = reader["PURCHASE_DATE"] as DateTime?,
                    SERIAL_NO = reader["SERIAL_NO"]?.ToString(),
                    COMPLAINT = reader["COMPLAINT"]?.ToString(),
                    SF_ID = reader["SF_ID"] != DBNull.Value ? Convert.ToInt32(reader["SF_ID"]) : (int?)null,

                    PROMISE_DATE = reader["PROMISE_DATE"] as DateTime?,
                    PROMISE_TIME = reader["PROMISE_TIME"]?.ToString(),
                    CATEGORY = reader["CATEGORY"]?.ToString(),
                    ENTERBY = reader["ENTERBY"]?.ToString(),
                    COMPLAINT_STATUS = reader["COMPLAINT_STATUS"]?.ToString(),
                    REMARKS = reader["REMARKS"]?.ToString(),
                    EMAIL = reader["EMAIL"]?.ToString(),
                    COMPLAINT_SUBSTATUS = reader["COMPLAINT_SUBSTATUS"]?.ToString(),

                    ENTEREDBY = reader["ENTEREDBY"]?.ToString(),
                    ENTEREDBYID = reader["ENTEREDBYID"] as int?,
                    IPADDRESS = reader["IPADDRESS"]?.ToString(),

                     Model = reader["Model"]?.ToString(),
                    ASC_CODE = reader["ASC_CODE"]?.ToString(),
                    Asc_Name = reader["ASC_NAME"]?.ToString(),
                    CancellationPreStatus = reader["CancellationPreStatus"]?.ToString(),
                    Bid = reader["Bid"] as int?,
                    RegionID = reader["RegionID"] as int?
                });
            }

            return complaints;
        }


        public async Task<bool> ApproveOrUpdateComplaintAsync(string ticketNo,int userId,string action,string? remarks,
            string? ascCode,string? dropdownSelection,string? complaintStatus,string? enteredBy,string? category)
            {
                using var connection = _sqlConnectionService.GetConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand("CC_ApproveCall", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // From ComplaintHistory
                command.Parameters.AddWithValue("@TicketNo", ticketNo);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Action", action);
                command.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
                command.Parameters.AddWithValue("@ASC_Code", (object?)ascCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@DropdownSelection", (object?)dropdownSelection ?? DBNull.Value);

                // From CallRegisterModel
                //command.Parameters.AddWithValue("@COMPLAINT_STATUS", (object?)complaintStatus ?? DBNull.Value);
                //command.Parameters.AddWithValue("@EnteredBy", (object?)enteredBy ?? DBNull.Value);
                //command.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                return rows > 0;
            }



        public async Task<List<CallRegisterModel>> UpdateComplaintAsync(string ticketNo, CallRegisterModel updatedModel)
        {
            var complaints = new List<CallRegisterModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_Get_ByTicketNo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            

            command.Parameters.AddWithValue("@type", "UpdateComplaint");
            command.Parameters.AddWithValue("@TicketNo", ticketNo);
            command.Parameters.AddWithValue("@cst", (object?)updatedModel.COMPLAINT_STATUS ?? DBNull.Value);
            command.Parameters.AddWithValue("@csst", (object?)updatedModel.COMPLAINT_SUBSTATUS ?? DBNull.Value);
            command.Parameters.AddWithValue("@histremark",(object?)updatedModel.dlcity ?? DBNull.Value);
            command.Parameters.AddWithValue("@username", (object?)updatedModel.CallTransferBY ?? DBNull.Value);
            command.Parameters.AddWithValue("@Dealer_ID", string.IsNullOrWhiteSpace(updatedModel.Dealer_ID?.Trim()) ? (object)DBNull.Value: updatedModel.Dealer_ID.Trim());
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                complaints.Add(new CallRegisterModel
                {
                    CallId = reader["CallId"] as int?,
                    SALUTATION = reader["SALUTATION"]?.ToString(),
                    FIRSTNAME = reader["FIRSTNAME"]?.ToString(),
                    LASTNAME = reader["LASTNAME"]?.ToString(),
                    HMPH = reader["HMPH"]?.ToString(),
                    WKPH = reader["WKPH"]?.ToString(),
                    STDCODE = reader["STDCODE"]?.ToString(),
                    PINCODE = reader["PINCODE"]?.ToString(),
                    MOBILE = reader["MOBILE"]?.ToString(),
                    ADDRESS1 = reader["ADDRESS1"]?.ToString(),
                    ADDRESS2 = reader["ADDRESS2"]?.ToString(),
                    CITY_ID = reader["CITY_ID"] != DBNull.Value ? Convert.ToInt32(reader["CITY_ID"]) : (int?)null,
                    TICKET_NO = reader["TICKET_NO"]?.ToString(),
                    LOCALITY_ID = reader["LOCALITY_ID"] != DBNull.Value ? Convert.ToInt32(reader["LOCALITY_ID"]) : (int?)null,
                    LANDMARK = reader["LANDMARK"]?.ToString(),
                    CALL_CATEGORY = reader["CALL_CATEGORY"]?.ToString(),
                    CALL_TYPEID = reader["CALL_TYPEID"] != DBNull.Value ? Convert.ToInt32(reader["CALL_TYPEID"]) : (int?)null,
                    CALLBY_ID = reader["CALLBY_ID"] != DBNull.Value ? Convert.ToInt32(reader["CALLBY_ID"]) : (int?)null,
                    WARRANTYID = reader["WARRANTYID"] != DBNull.Value ? Convert.ToInt32(reader["WARRANTYID"]) : (int?)null,
                    dlcity = reader["dlcity"]?.ToString(),
                    CALL_TYPE = reader["CALL_TYPE"]?.ToString(),
                    CALL_BY = reader["CALL_BY"]?.ToString(),
                    DEALER = reader["DEALER"]?.ToString(),
                    Dealer_ID = reader["Dealer_ID"]?.ToString(),
                    WARRANTY = reader["WARRANTY"]?.ToString(),
                    PRODUCT = reader["PRODUCT"]?.ToString(),
                    PRODUCT_ID = reader["PRODUCT_ID"] != DBNull.Value ? Convert.ToInt32(reader["PRODUCT_ID"]) : (int?)null,
                    MODEL_ID = reader["MODEL_ID"] != DBNull.Value ? Convert.ToInt32(reader["MODEL_ID"]) : (int?)null,
                    PURCHASE_DATE = reader["PURCHASE_DATE"] as DateTime?,
                    SERIAL_NO = reader["SERIAL_NO"]?.ToString(),
                    COMPLAINT = reader["COMPLAINT"]?.ToString(),
                    SF_ID = reader["SF_ID"] != DBNull.Value ? Convert.ToInt32(reader["SF_ID"]) : (int?)null,

                    PROMISE_DATE = reader["PROMISE_DATE"] as DateTime?,
                    PROMISE_TIME = reader["PROMISE_TIME"]?.ToString(),
                    CATEGORY = reader["CATEGORY"]?.ToString(),
                    ENTERBY = reader["ENTERBY"]?.ToString(),
                    COMPLAINT_STATUS = reader["COMPLAINT_STATUS"]?.ToString(),
                    REMARKS = reader["REMARKS"]?.ToString(),
                    EMAIL = reader["EMAIL"]?.ToString(),
                    COMPLAINT_SUBSTATUS = reader["COMPLAINT_SUBSTATUS"]?.ToString(),

                    ENTEREDBY = reader["ENTEREDBY"]?.ToString(),
                    ENTEREDBYID = reader["ENTEREDBYID"] as int?,
                    IPADDRESS = reader["IPAddress"]?.ToString(),
                    CallTransferBY = reader["CallTransferBY"]?.ToString(),
                    Model = reader["Model"]?.ToString(),
                    ASC_CODE = reader["ASC_CODE"]?.ToString(),
                    Asc_Name = reader["ASC_NAME"]?.ToString(),
                    CancellationPreStatus = reader["CancellationPreStatus"]?.ToString(),
                    Bid = reader["Bid"] as int?,
                    RegionID = reader["RegionID"] as int?
                });
            }

            return complaints;
        }

        // 🆕 PRIVATE HELPER: Save Request Payload JSON into SQL Table
        // ==============================================================
        //public async Task SaveRequestPayloadAsync(object payload, string source)
        //{
        //    try
        //    {
        //        // Convert model object to JSON string
        //        string jsonData = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        //        {
        //            WriteIndented = false,
        //            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        //        });

        //        using var connection = _sqlConnectionService.GetConnection();
        //        await connection.OpenAsync();

        //        using var command = new SqlCommand("ComplaintPayload_Save", connection)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        command.Parameters.AddWithValue("@PayloadJson", jsonData);
        //        command.Parameters.AddWithValue("@Source", source ?? (object)DBNull.Value);

        //        await command.ExecuteNonQueryAsync();
        //    }
        //    catch
        //    {
        //        // ⚠️ Intentionally ignored — do not break complaint creation
        //    }
        //}









    }
}
