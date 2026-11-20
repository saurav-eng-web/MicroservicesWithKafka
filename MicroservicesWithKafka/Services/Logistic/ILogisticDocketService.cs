using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticDocketService
    {
        Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search);
        Task<(bool Success, string Message)> DeleteDocketAsync(int id);

        /// <summary>Resolves a StatusView code (e.g. "DefectivePart") or legacy display name (e.g. "Defective Part") to the display name the SPs expect; null when unknown.</summary>
        string? ResolveChallanType(string? type);

        Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo);
        Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo);

        Task<(bool Success, string Message)> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId);
        Task<(bool Success, string Message)> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows);

        /// <summary>Forward Logistic Docket Uploader. NotApplied = rows that changed nothing (challan / docket not found, bad status).</summary>
        Task<(bool Success, string Message, IReadOnlyList<ForwardDocketNotAppliedViewModel> NotApplied)> UploadForwardDocketsAsync(
            IEnumerable<ForwardDocketUploadViewModel> rows, string userName);

        Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync();
        Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync();
        Task<(bool Success, string Message)> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows);
    }
}
