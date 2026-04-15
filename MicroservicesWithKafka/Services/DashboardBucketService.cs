using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class DashboardBucketService : IDashboardBucketService
    {
        private readonly IDashboardBucketRepository _repository;

        public DashboardBucketService(IDashboardBucketRepository repository)
        {
            _repository = repository;
        }

        public Task<List<DashboardBucket>> GetAllAsync() => _repository.GetAllAsync();

        public Task<DashboardBucket> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

        public Task<List<DashboardBucket>> GetByRoleAsync(int role) => _repository.GetByRoleAsync(role);

        public Task CreateAsync(DashboardBucket bucket) => _repository.CreateAsync(bucket);

        public Task UpdateAsync(string id, DashboardBucket bucket) => _repository.UpdateAsync(id, bucket);

        public Task DeleteAsync(string id) => _repository.DeleteAsync(id);

        public Task<List<ComplaintAggregationResult>> GetComplaintAggregationAsync(int bid,int role) =>
            _repository.GetComplaintAggregationAsync(bid,role);
    }
}
