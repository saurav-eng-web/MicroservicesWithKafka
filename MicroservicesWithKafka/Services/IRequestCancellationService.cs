using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IRequestCancellationService
    {
        Task<RequestCancellation> CreateAsync(RequestCancellation model);
        Task<RequestCancellation> UpdateAsync(RequestCancellation model);
        Task<RequestCancellation> DeleteAsync(long id);
        Task<IEnumerable<RequestCancellation>> GetAllAsync();
        Task<RequestCancellation?> GetByIdAsync(long id);
    }
}
