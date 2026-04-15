using MicroservicesWithKafka.Models;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IComplaintCustDetailRepository
    {
        Task<ComplaintCustDetail?> GetByTicketNoAsync(string ticketNo);
    }
}
