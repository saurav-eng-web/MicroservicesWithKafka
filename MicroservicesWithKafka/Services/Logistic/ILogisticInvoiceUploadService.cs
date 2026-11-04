using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticInvoiceUploadService
    {
        Task<(bool Success, string Message)> UploadInvoicePoAsync(IEnumerable<LogisticInvoicePoUploadViewModel> rows, string userName);
        Task<(bool Success, string Message)> UploadInvoiceRateAsync(IEnumerable<LogisticInvoiceRateUploadViewModel> rows, string userName);
    }
}
