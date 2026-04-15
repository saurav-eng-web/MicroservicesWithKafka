using MicroservicesWithKafka.Models;
using System.Xml;
using Serilog;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class FundRepository : IFundRepository
    {
        private readonly string filePath = "Data/fundDetails.json";
        private readonly IMongoCollection<Fund> _fundsCollection;

        public FundRepository(IOptions<FundsDatabaseSettings> fundsDatabaseSettings)
        {
            var mongoClient = new MongoClient(fundsDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(fundsDatabaseSettings.Value.DatabaseName);
            _fundsCollection = mongoDatabase.GetCollection<Fund>(fundsDatabaseSettings.Value.CollectionName);
        }

        public Task<List<Fund>> GetAllFunds()
        {
            Log.Information("Fetching all funds from JSON file.");
            return _fundsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<(List<Fund> Items, int TotalCount)> GetPagedFunds(int page, int pageSize)
        {
            var filter = Builders<Fund>.Filter.Empty;
            var totalCount = await _fundsCollection.CountDocumentsAsync(filter);

            var items = await _fundsCollection.Find(filter)
                                              .Skip((page - 1) * pageSize)
                                              .Limit(pageSize)
                                              .ToListAsync();

            return (items, (int)totalCount);
        }

        public Task<Fund> GetFundByID(int id)
        {
            Log.Information($"Fetching fund with ID: {id}");
            return _fundsCollection.Find(x => x.FundId == id).FirstOrDefaultAsync();
        }

        public async Task<List<Fund>> FilterFundsByField(string fieldName, string fieldValue)
        {
            try
            {
                var filterBuilder = Builders<Fund>.Filter;
                FilterDefinition<Fund> filter;

                switch (fieldName.ToLower())
                {
                    case "fundid":
                        if (int.TryParse(fieldValue, out int fundIdValue))
                            filter = filterBuilder.Eq(f => f.FundId, fundIdValue);
                        else
                            filter = filterBuilder.Empty;
                        break;

                    case "fundname":
                        filter = filterBuilder.Regex(f => f.FundName, new MongoDB.Bson.BsonRegularExpression(fieldValue, "i"));
                        break;

                    case "fundobjective":
                        filter = filterBuilder.Regex(f => f.FundObjective, new MongoDB.Bson.BsonRegularExpression(fieldValue, "i"));
                        break;

                    default:
                        filter = filterBuilder.Regex(fieldName, new MongoDB.Bson.BsonRegularExpression(fieldValue, "i"));
                        break;
                }

                var items = await _fundsCollection.Find(filter).ToListAsync();
                return items;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Error filtering funds by field: {ex.Message}");
                throw;
            }
        }

        // 🔧 ERROR FIXED: changed from void → async Task
        public async Task AddFund(Fund fund)
        {
            await _fundsCollection.InsertOneAsync(fund);
            Log.Information($"Fund added: {JsonConvert.SerializeObject(fund)}");
        }

        // 🔧 ERROR FIXED: changed from void → async Task
        public async Task UpdateFund(Fund fund)
        {
            var existingFund = await _fundsCollection.Find(x => x.FundId == fund.FundId).FirstOrDefaultAsync();
            if (existingFund != null)
            {
                await _fundsCollection.ReplaceOneAsync(x => x.FundId == fund.FundId, fund);
                Log.Information($"Fund updated: {JsonConvert.SerializeObject(fund)}");
            }
        }

        // 🔧 ERROR FIXED: changed from void → async Task
        public async Task DeleteFund(int id)
        {
            await _fundsCollection.DeleteOneAsync(x => x.FundId == id);
            Log.Information($"Fund deleted with ID: {id}");
        }

        private List<Fund> ReadFromFile()
        {
            if (!File.Exists(filePath)) return new List<Fund>();
            var jsonData = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Fund>>(jsonData) ?? new List<Fund>();
        }

        private void WriteToFile(List<Fund> funds)
        {
            var jsonData = JsonConvert.SerializeObject(funds, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
