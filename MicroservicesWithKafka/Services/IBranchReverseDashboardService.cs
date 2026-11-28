using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IBranchReverseDashboardService
    {
        // scope (branch or ASC) comes from the login token; branchId / sfId in the payload are ignored
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, DashboardScope scope);
    }
}
