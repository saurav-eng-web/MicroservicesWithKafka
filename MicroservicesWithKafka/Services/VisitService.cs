using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepository _repo;
        public VisitService(IVisitRepository repo) => _repo = repo;

        public Task<IEnumerable<dynamic>> CreateVisitAsync(Visit visit) => _repo.CreateVisitAsync(visit);
        public Task<IEnumerable<dynamic>> UpdateVisitAsync(Visit visit) => _repo.UpdateVisitAsync(visit);
        public Task<IEnumerable<dynamic>> UpdateVisitByTicketAsync(Visit visit)=> _repo.UpdateVisitByTicketAsync(visit);

        public Task<IEnumerable<dynamic>> DeleteVisitAsync(long slNo) => _repo.DeleteVisitAsync(slNo);
        public Task<IEnumerable<dynamic>> GetVisitByIdAsync(long slNo) => _repo.GetVisitByIdAsync(slNo);
        public Task<IEnumerable<dynamic>> GetAllVisitsAsync() => _repo.GetAllVisitsAsync();
        public Task<IEnumerable<dynamic>> GetVisitsByTicketNoAsync(string ticketNo) => _repo.GetVisitsByTicketNoAsync(ticketNo);
    }
}
