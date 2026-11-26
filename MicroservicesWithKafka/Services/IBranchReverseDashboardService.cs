using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IBranchReverseDashboardService
    {
        // branchId comes from the login token; a branchId in the payload is ignored
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, int branchId);
    }
}
