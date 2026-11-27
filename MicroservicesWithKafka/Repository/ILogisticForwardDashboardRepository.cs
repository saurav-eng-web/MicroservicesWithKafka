using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticForwardDashboardRepository
    {
        Task<ForwardDashboardSummaryViewModel> GetSummaryAsync();
        Task<IEnumerable<Dictionary<string, string>>> GetDetailsAsync(string bucket);
        Task<IEnumerable<Dictionary<string, string>>> GetChallanAsync(string challanNo);
    }
}
