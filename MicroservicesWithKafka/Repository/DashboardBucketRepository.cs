using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services.Mongo;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class DashboardBucketRepository : IDashboardBucketRepository
    {
        private readonly IMongoCollection<DashboardBucket> _bucketCollection;
        private readonly IMongoCollection<CallRegisterModel> _callCollection;
        public DashboardBucketRepository(MongoDBService mongoDbService)
        {
            // use your wrapper service to get the collection
            _bucketCollection = mongoDbService.GetCollection<DashboardBucket>("dashboardBuckets");

            // Call register collection (for complaints)
            _callCollection = mongoDbService.GetCollection<CallRegisterModel>("CallRegister");
        }

        public async Task<List<DashboardBucket>> GetAllAsync() =>
            await _bucketCollection.Find(_ => true).ToListAsync();

        public async Task<DashboardBucket> GetByIdAsync(string id) =>
            await _bucketCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<DashboardBucket>> GetByRoleAsync(int role) =>
            await _bucketCollection.Find(x => x.Role == role).ToListAsync();

        public async Task CreateAsync(DashboardBucket bucket) =>
            await _bucketCollection.InsertOneAsync(bucket);

        public async Task UpdateAsync(string id, DashboardBucket bucket) =>
            await _bucketCollection.ReplaceOneAsync(x => x.Id == id, bucket);

        public async Task DeleteAsync(string id) =>
            await _bucketCollection.DeleteOneAsync(x => x.Id == id);


        // 🔥 New method for aggregation
        public async Task<List<ComplaintAggregationResult>> GetComplaintAggregationAsync(int bid, int role)
        {
            FilterDefinition<CallRegisterModel> filter;

            if (role == 2) // role 2 → filter by Bid
            {
                filter = Builders<CallRegisterModel>.Filter.Eq(x => x.Bid, bid);
            }
            else if (role == 3) // role 3 → filter by SF_ID
            {
                filter = Builders<CallRegisterModel>.Filter.Eq(x => x.SF_ID, bid);
            }
            else if (role == 8) // role 8 → filter by ASMID
            {
                filter = Builders<CallRegisterModel>.Filter.Eq(x => x.ASMID, bid);
            }
            else if (role == 9) // role 9 → filter by RegionID
            {
                filter = Builders<CallRegisterModel>.Filter.Eq(x => x.RegionID, bid);
            }
            else // any other role → no filter (all records)
            {
                filter = Builders<CallRegisterModel>.Filter.Empty;
            }

            var result = await _callCollection.Aggregate()
                .Match(filter)
                .Group(x => new { x.COMPLAINT_STATUS, x.COMPLAINT_SUBSTATUS },
                       g => new ComplaintAggregationResult
                       {
                           ComplaintStatus = g.Key.COMPLAINT_STATUS,
                           ComplaintSubStatus = g.Key.COMPLAINT_SUBSTATUS,
                           Count = g.Count()
                       })
                .ToListAsync();

            return result;
        }


    }
}
