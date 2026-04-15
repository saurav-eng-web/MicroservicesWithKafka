using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class NewCallRegisterRepository : INewCallRegisterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public NewCallRegisterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<string?> CreateComplaintAsync(NewCallRegisterModel model)
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
            command.Parameters.AddWithValue("@COMPLAINT_STATUS", model.COMPLAINT_STATUS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", model.REMARKS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", model.EMAIL ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_SubStatus", model.COMPLAINT_SUBSTATUS ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Excalated", model.EXCALATED ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Legal", model.LEGAL ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredBy", model.ENTEREDBY ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredByID", model.ENTEREDBYID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IPAddress", model.IPADDRESS ?? (object)DBNull.Value);

            // Execute the stored procedure and read the result from SELECT
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Assuming stored procedure returns: SELECT @ReturnNo AS ReturnNo
                return reader["ReturnNo"]?.ToString();
            }

            return null;
        }

    }
}
