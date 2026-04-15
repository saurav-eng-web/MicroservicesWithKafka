using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IDashboardBucketService
    {
        Task<List<DashboardBucket>> GetAllAsync();
        Task<DashboardBucket> GetByIdAsync(string id);
        Task<List<DashboardBucket>> GetByRoleAsync(int role);
        Task CreateAsync(DashboardBucket bucket);
        Task UpdateAsync(string id, DashboardBucket bucket);
        Task DeleteAsync(string id);

        Task<List<ComplaintAggregationResult>> GetComplaintAggregationAsync( int bid,int role);
    }
}
