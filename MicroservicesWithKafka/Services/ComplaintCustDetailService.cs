using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class ComplaintCustDetailService : IComplaintCustDetailService
    {
        private readonly IComplaintCustDetailRepository _repository;

        public ComplaintCustDetailService(IComplaintCustDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<ComplaintCustDetail?> GetByTicketNoAsync(string ticketNo)
        {
            // 👉 You can add business logic here before/after fetching from repository
            return await _repository.GetByTicketNoAsync(ticketNo);
        }
    }
}
