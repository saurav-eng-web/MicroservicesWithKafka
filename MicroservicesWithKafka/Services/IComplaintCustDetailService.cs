using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IComplaintCustDetailService
    {
        Task<ComplaintCustDetail?> GetByTicketNoAsync(string ticketNo);
    }
}
