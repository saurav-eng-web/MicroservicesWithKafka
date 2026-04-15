using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ICallCreationUploaderRepository
    {
        Task<List<int>> CreateBulkAsync(List<CallCreationUploader> models);
        Task<string> UpdateAsync(CallCreationUploader model);
        Task<string> DeleteAsync(int id);
        Task<IEnumerable<CallCreationUploader>> GetAllAsync();
        Task<CallCreationUploader?> GetByIdAsync(int id);
    }
}
