using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticCnRepository
    {
        Task<int> UploadAsync(IEnumerable<CnLogisticUploadViewModel> rows, string userId);
        Task<IEnumerable<dynamic>> GetUploadedCnAsync();
        Task<IEnumerable<dynamic>> GetChallanByTicketAsync(string ticketNo);
    }
}
