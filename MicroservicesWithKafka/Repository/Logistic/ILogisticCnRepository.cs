using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>Legacy Logistic/CNLogisticUploader.aspx.vb</summary>
    public interface ILogisticCnRepository
    {
        Task<int> UploadAsync(IEnumerable<CnLogisticUploadViewModel> rows, string userId);
        Task<IEnumerable<dynamic>> GetUploadedCnAsync();
        Task<IEnumerable<dynamic>> GetChallanByTicketAsync(string ticketNo);
    }
}
