using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IComplaintHistoryRepository
    {

        Task<long> InsertAsync(ComplaintHistory model); // Add complaint
        Task<bool> UpdateAsync(ComplaintHistory model);
        Task<bool> DeleteAsync(long id);
        Task<List<ComplaintHistory>> GetAllAsync();
        Task<ComplaintHistory?> GetByIdAsync(long id);

        Task<List<ComplaintHistory>> GetAllByTicketNoAsync(string ticketNo);
    }
}