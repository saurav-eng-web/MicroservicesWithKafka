using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticDocketRepository
    {
        Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search);
        Task<bool> DeleteDocketAsync(int id);

        Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo);
        Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo);

        Task<int> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId);
        Task<int> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows);

        Task<ForwardDocketUploadResult> UploadForwardDocketsAsync(IEnumerable<ForwardDocketRow> rows, string userName);

        Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync();

        Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync();

        Task<int> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows);
    }
}
