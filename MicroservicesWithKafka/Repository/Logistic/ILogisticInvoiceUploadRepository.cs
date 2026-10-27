using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    /// <summary>Legacy GST/LogisticInvoiceUploader.aspx.vb and GST/LogisticInvoiceRateUploader.aspx.vb (Logistic1 menu).</summary>
    public interface ILogisticInvoiceUploadRepository
    {
        Task<int> UploadInvoicePoAsync(IEnumerable<LogisticInvoicePoUploadViewModel> rows, string userName);
        Task<int> UploadInvoiceRateAsync(IEnumerable<LogisticInvoiceRateUploadViewModel> rows, string userName);
    }
}
