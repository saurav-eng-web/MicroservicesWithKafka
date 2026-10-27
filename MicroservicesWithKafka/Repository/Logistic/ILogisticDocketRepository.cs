using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticDocketRepository
    {
        // DeleteDocket.aspx.vb
        Task<IEnumerable<DocketViewModel>> GetDocketsAsync(string search);
        Task<bool> DeleteDocketAsync(int id);

        // History.aspx.vb / SearchHistory.aspx.vb — null when the SP returns no header row
        Task<DocketDetailViewModel?> GetDocketDetailAsync(string type, string challanNo);
        Task<DocketDetailViewModel?> SearchDocketAsync(string type, string challanNo);

        // DocketChange.aspx.vb / DocketChangeStatus.aspx.vb — return number of rows imported
        Task<int> UploadDocketChangeAsync(IEnumerable<DocketViewModel> rows, string userId);
        Task<int> UploadDocketChangeStatusAsync(IEnumerable<DocketViewModel> rows);

        // DocketUpload.aspx.vb DownloadDocket_Click
        Task<IEnumerable<DocketDownloadViewModel>> GetDocketDownloadsAsync();

        // DocketUpload.aspx.vb DocketNot_Click
        Task<IEnumerable<DocketNotUploadedViewModel>> GetDocketsNotUploadedAsync();

        // DocketUpload.aspx.vb bindgrid
        Task<int> UploadDocketsAsync(IEnumerable<DocketUploadRequestViewModel> rows);
    }
}
