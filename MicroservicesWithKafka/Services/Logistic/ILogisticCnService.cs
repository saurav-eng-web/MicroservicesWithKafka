using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticCnService
    {
        Task<(bool Success, string Message)> UploadAsync(IEnumerable<CnLogisticUploadViewModel> rows, string userId);
        Task<IEnumerable<dynamic>> SearchAsync(string? ticketNo);
    }
}
