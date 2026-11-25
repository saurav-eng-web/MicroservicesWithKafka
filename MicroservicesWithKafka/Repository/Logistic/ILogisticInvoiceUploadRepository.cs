using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticInvoiceUploadRepository
    {
        Task<int> UploadInvoicePoAsync(IEnumerable<LogisticInvoicePoUploadViewModel> rows, string userName);
        Task<int> UploadInvoiceRateAsync(IEnumerable<LogisticInvoiceRateUploadViewModel> rows, string userName);
    }
}
