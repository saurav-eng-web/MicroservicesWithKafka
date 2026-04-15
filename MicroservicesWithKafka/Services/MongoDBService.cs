using MicroservicesWithKafka.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MicroservicesWithKafka.Enums;


namespace MicroservicesWithKafka.Services.Mongo
{
    public class MongoDBService
    {
        private readonly IMongoDatabase database;
        private readonly IMongoCollection<CallRegisterModel> _callCollection;
        private readonly IMongoCollection<CustomerMongoModel> _customerCollection;
        private readonly IMongoCollection<DashboardBucket> _DashboardBucket;
        private readonly IMongoCollection<ComplaintHistory> _calhistoryCollection;
        public MongoDBService(IConfiguration configuration)
        {
            var connectionString = configuration["CallRegisterDatabase:ConnectionString"];
            var databaseName = configuration["CallRegisterDatabase:DatabaseName"];
            var callCollectionName = configuration["CallRegisterDatabase:CollectionName"];

            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);  

            _callCollection = database.GetCollection<CallRegisterModel>(callCollectionName);
            _customerCollection = database.GetCollection<CustomerMongoModel>("CustomerMaster");
            _calhistoryCollection= database.GetCollection<ComplaintHistory>("CallHistory");
            _DashboardBucket = database.GetCollection<DashboardBucket>("DashboardBucket");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return database.GetCollection<T>(collectionName); 
        }

        /// <summary>
        /// Inserts a call record into MongoDB.
        /// </summary>
        public async Task InsertCallAsync(CallRegisterModel model, string returnNo)
        {
            // Attach ReturnNo to Call Register document
            model.TICKET_NO = returnNo;

            await _callCollection.InsertOneAsync(model);



        }

        public async Task InsertCallhistoryAsync(ComplaintHistory model)
        {
            // Attach ReturnNo to Call Register document
            var complainthistory = new ComplaintHistory
            {
                TicketNo = model.TicketNo,
                ComplaintSubStatus = model.ComplaintSubStatus,
                ComplaintStatus = model.ComplaintStatus,
                EnteredBy = model.EnteredBy,
                Tdate = DateTime.Now,
                EnteredByID = model.EnteredByID

            };
                
            await _calhistoryCollection.InsertOneAsync(complainthistory);
        }

        /// <summary>
        /// Inserts a customer record into MongoDB.
        /// </summary>
        public async Task InsertCustomerAsync(CallRegisterModel model, string? TICKET_NO = null)
        {
            var customer = new CustomerMongoModel
            {
                SALUTATION = model.SALUTATION,
                FIRSTNAME = model.FIRSTNAME,
                LASTNAME = model.LASTNAME,
                MOBILE = model.MOBILE,
                EMAIL = model.EMAIL,
                ADDRESS1 = model.ADDRESS1,
                ADDRESS2 = model.ADDRESS2,
                //CityId = model.CITY_ID,
                PINCODE = model.PINCODE,
                LANDMARK = model.LANDMARK,
                CreatedAt = DateTime.UtcNow,
                TICKET_NO = TICKET_NO
            };

            await _customerCollection.InsertOneAsync(customer);
        }


        /// <summary>
        /// Inserts both Call Register and Customer in MongoDB in one go.
        /// </summary>
        public async Task InsertCallAndCustomerAsync(CallRegisterModel model, string returnNo)
        {
            // Save call info
            await InsertCallAsync(model, returnNo);

            // Save customer info
            await InsertCustomerAsync(model);
        }

        public IMongoCollection<dynamic> GetErrorLogCollection()
        {
            var database = _callCollection.Database;
            return database.GetCollection<dynamic>("KafkaErrorLogs");
        }

        //  ======   This is for only call center use 

        //public async Task<List<CallRegisterModel>> SearchCallsAsync(string? serialNo, string? callNumber, string? phone)
        //{
        //    var filters = new List<FilterDefinition<CallRegisterModel>>();

        //    if (!string.IsNullOrEmpty(serialNo))
        //        filters.Add(Builders<CallRegisterModel>.Filter.Regex(x => x.SERIAL_NO, new BsonRegularExpression(serialNo, "i")));

        //    if (!string.IsNullOrEmpty(callNumber))
        //        filters.Add(Builders<CallRegisterModel>.Filter.Regex("TICKET_NO", new BsonRegularExpression(callNumber, "i")));

        //    if (!string.IsNullOrEmpty(phone))
        //        filters.Add(Builders<CallRegisterModel>.Filter.Regex(x => x.MOBILE, new BsonRegularExpression(phone, "i")));

        //    FilterDefinition<CallRegisterModel> filter;

        //    if (filters.Count > 0)
        //        filter = Builders<CallRegisterModel>.Filter.Or(filters);  // ✅ Match ANY parameter
        //    else
        //        filter = Builders<CallRegisterModel>.Filter.Empty;

        //    return await _callCollection.Find(filter).ToListAsync();
        //}

        public async Task<List<CallSearchResponseModel>> SearchCallsAsync(string? searchText,ScopeType scopeType,int scopeId)

        {
            var builder = Builders<CallRegisterModel>.Filter;
            var filters = new List<FilterDefinition<CallRegisterModel>>();

            // 🔐 Apply scope filter
            if (scopeId > 0)
            {
                switch (scopeType)
                {
                    case ScopeType.Bid:
                        filters.Add(builder.Eq(x => x.Bid, scopeId));
                        break;

                    case ScopeType.Region:
                        filters.Add(builder.Eq(x => x.RegionID, scopeId));
                        break;

                    case ScopeType.Asm:
                        filters.Add(builder.Eq(x => x.ASMID, scopeId));
                        break;

                    case ScopeType.Sf:
                        filters.Add(builder.Eq(x => x.SF_ID, scopeId));
                        break;
                }
            }

            // 🔍 Single search input → multiple fields
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var regex = new BsonRegularExpression(searchText.Trim(), "i");

                filters.Add(builder.Or(
                    builder.Regex(x => x.SERIAL_NO, regex),
                    builder.Regex(x => x.TICKET_NO, regex),
                    builder.Regex(x => x.MOBILE, regex)
                ));
            }

            var finalFilter = filters.Count > 0
                ? builder.And(filters)
                : builder.Empty;

            return await _callCollection
             .Find(finalFilter)
             .Project(call => new CallSearchResponseModel
             {
                 SALUTATION = call.SALUTATION,
                 FIRSTNAME = call.FIRSTNAME,
                 LASTNAME = call.LASTNAME,
//<<<<<<< Updated upstream
                
//=======
                 HMPH = call.MOBILE,
                 WKPH = call.HMPH,
//>>>>>>> Stashed changes
                 Model = call.Model,
                 TICKET_NO = call.TICKET_NO,
                 SERIAL_NO = call.SERIAL_NO,
                 CALL_TYPE = call.CALL_TYPE,
                 COMPLAINT_SUBSTATUS = call.COMPLAINT_SUBSTATUS,
                 PRODUCT = call.PRODUCT
             })
             .ToListAsync();

        }

        /// <summary>
        /// Upserts a Call Register record in MongoDB.
        /// Updates if exists (based on TICKET_NO), inserts if not.
        /// </summary>
        public async Task<bool> UpsertCallAsync(string ticketNo, CallRegisterModel updatedModel)
        {
            // Ensure TICKET_NO is set on the updated model
            updatedModel.TICKET_NO = ticketNo;

            var filter = Builders<CallRegisterModel>.Filter.Eq(c => c.TICKET_NO, ticketNo);

            var update = Builders<CallRegisterModel>.Update
                .Set(c => c.COMPLAINT_STATUS, updatedModel.COMPLAINT_STATUS)
                .Set(c => c.COMPLAINT_SUBSTATUS, updatedModel.COMPLAINT_SUBSTATUS)
                .Set(c => c.Model, updatedModel.Model)
                .Set(c => c.SERIAL_NO, updatedModel.SERIAL_NO)
                .Set(c => c.CALL_TYPE, updatedModel.CALL_TYPE)
                .Set(c => c.PRODUCT, updatedModel.PRODUCT)
                .Set(c => c.ASMID, updatedModel.ASMID)
                .Set(c => c.RegionID, updatedModel.RegionID)
                .Set(c => c.SF_ID, updatedModel.SF_ID)
                .Set(c => c.Bid, updatedModel.Bid)
                .Set(c => c.Asc_Name, updatedModel.Asc_Name)
                .Set(c => c.MODEL_ID, updatedModel.MODEL_ID)
                .Set(c => c.WARRANTYID, updatedModel.WARRANTYID)
                .Set(c => c.WARRANTY, updatedModel.WARRANTY);
            // .Set(c => c.UPDATED_AT, DateTime.UtcNow); // optional, if you track update time

            var result = await _callCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });

            return result.ModifiedCount > 0;
        }




    }
}
