using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IComplaintHistoryService
    {
        Task<long> AddComplaintAsync(ComplaintHistory model); // Add complaint and send Kafka
        Task<bool> UpdateComplaintAsync(ComplaintHistory model);
        Task<bool> DeleteComplaintAsync(long id);
        Task<List<ComplaintHistory>> GetAllComplaintsAsync();
        Task<ComplaintHistory?> GetComplaintByIdAsync(long id);
        Task<List<ComplaintHistory>> GetAllComplaintsByTicketNoAsync(string ticketNo);

    }
}
