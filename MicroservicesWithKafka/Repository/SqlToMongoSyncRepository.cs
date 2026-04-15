using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MicroservicesWithKafka.Repository
{
    public class SqlToMongoSyncRepository : ISqlToMongoSyncRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;
        private readonly IMongoCollection<CallRegisterModel> _mongoCollection;

        public SqlToMongoSyncRepository(SQLConnectionService sqlConnectionService, IConfiguration config)
        {
            _sqlConnectionService = sqlConnectionService;

            // ✅ Load Mongo settings from appsettings.json
            var mongoSection = config.GetSection("CallRegisterDatabase");
            var mongoConn = mongoSection["ConnectionString"];
            var mongoDb = mongoSection["DatabaseName"];
            var collectionName = mongoSection["CollectionName"];

            var mongoClient = new MongoClient(mongoConn);
            var database = mongoClient.GetDatabase(mongoDb);
            _mongoCollection = database.GetCollection<CallRegisterModel>(collectionName);
        }

        public async Task<int> SyncSqlToMongoAsync(DateTime? fromDate, DateTime? toDate, int? regionId)
        {
            int count = 0;

            using var sqlConn = _sqlConnectionService.GetConnection();
            await sqlConn.OpenAsync();

            using var cmd = new SqlCommand("GetCallRegisterForSyncByFilter", sqlConn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FromDate", (object?)fromDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ToDate", (object?)toDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RegionID", (object?)regionId ?? DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var model = new CallRegisterModel
                {
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
                    CITY_ID = reader["CITY_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["CITY_ID"]),
                    LOCALITY_ID = reader["LOCALITY_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["LOCALITY_ID"]),
                    LANDMARK = reader["LANDMARK"]?.ToString(),
                    CALL_CATEGORY = reader["CALL_CATEGORY"]?.ToString(),
                    CALL_TYPEID = reader["CALL_TYPEID"] == DBNull.Value ? null : Convert.ToInt64(reader["CALL_TYPEID"]),
                    CALLBY_ID = reader["CALLBY_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["CALLBY_ID"]),
                    WARRANTYID = reader["WARRANTYID"] == DBNull.Value ? null : Convert.ToInt64(reader["WARRANTYID"]),
                    CALL_TYPE = reader["CALL_TYPE"]?.ToString(),
                    CALL_BY = reader["CALL_BY"]?.ToString(),
                    DEALER = reader["DEALER"]?.ToString(),
                    WARRANTY = reader["WARRANTY"]?.ToString(),
                    PRODUCT = reader["PRODUCT"]?.ToString(),
                    PRODUCT_ID = reader["PRODUCT_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["PRODUCT_ID"]),
                    MODEL_ID = reader["MODEL_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["MODEL_ID"]),
                    PURCHASE_DATE = reader["PURCHASE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["PURCHASE_DATE"]),
                    SERIAL_NO = reader["SERIAL_NO"]?.ToString(),
                    COMPLAINT = reader["COMPLAINT"]?.ToString(),
                    SF_ID = reader["SF_ID"] == DBNull.Value ? null : Convert.ToInt64(reader["SF_ID"]),
                    PROMISE_DATE = reader["PROMISE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["PROMISE_DATE"]),
                    PROMISE_TIME = reader["PROMISE_TIME"]?.ToString(),
                    CATEGORY = reader["CATEGORY"]?.ToString(),
                    ENTERBY = reader["ENTERBY"]?.ToString(),
                    COMPLAINT_STATUS = reader["COMPLAINT_STATUS"]?.ToString(),
                    REMARKS = reader["REMARKS"]?.ToString(),
                    EMAIL = reader["EMAIL"]?.ToString(),
                    COMPLAINT_SUBSTATUS = reader["COMPLAINT_SUBSTATUS"]?.ToString(),
                    EXCALATED = reader["EXCALATED"] == DBNull.Value ? null : Convert.ToInt64(reader["EXCALATED"]),
                    LEGAL = reader["LEGAL"] == DBNull.Value ? null : Convert.ToInt64(reader["LEGAL"]),
                    ENTEREDBY = reader["ENTEREDBY"]?.ToString(),
                    ENTEREDBYID = reader["ENTEREDBYID"] == DBNull.Value ? null : Convert.ToInt64(reader["ENTEREDBYID"]),
                    IPADDRESS = reader["IPADDRESS"]?.ToString(),
                    CancellationPreStatus = reader["CancellationPreStatus"]?.ToString(),
                    Bid = reader["Bid"] == DBNull.Value ? null : Convert.ToInt32(reader["Bid"]),
                    RegionID = reader["RegionID"] == DBNull.Value ? null : Convert.ToInt32(reader["RegionID"]),
                    ASMID = reader["ASMID"] == DBNull.Value ? null : Convert.ToInt32(reader["ASMID"]),
                    CallId = reader["CallId"] == DBNull.Value ? null : Convert.ToInt64(reader["CallId"]),
                    CallNumber = reader["CallNumber"]?.ToString(),
                    TICKET_NO = reader["TICKET_NO"]?.ToString(),
                    State = reader["State"]?.ToString(),
                    City = reader["City"]?.ToString(),
                    Locality = reader["Locality"]?.ToString(),
                    ProductCategory = reader["ProductCategory"]?.ToString(),
                    Brand = reader["Brand"]?.ToString(),
                    POP = reader["POP"] == DBNull.Value ? null : Convert.ToDateTime(reader["POP"]),
                    Model = reader["Model"]?.ToString(),
                    FaultVoc = reader["FaultVoc"]?.ToString(),
                    AvailableTime = reader["AvailableTime"]?.ToString(),
                    Asc_Name = reader["Asc_Name"]?.ToString(),
                    ASC_CODE = reader["ASC_CODE"]?.ToString(),
                    ImageUpload = reader["ImageUpload"]?.ToString(),
                    UserId = reader["UserId"] == DBNull.Value ? null : Convert.ToInt32(reader["UserId"])
                };

                // ✅ UPSERT into Mongo
                var filter = Builders<CallRegisterModel>.Filter.Eq(x => x.CallId, model.CallId);
                var options = new ReplaceOptions { IsUpsert = true };
                await _mongoCollection.ReplaceOneAsync(filter, model, options);

                count++;
            }

            return count;
        }
    }
}
