using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IVisitRepository
    {
        Task<IEnumerable<dynamic>> CreateVisitAsync(Visit visit);
        Task<IEnumerable<dynamic>> UpdateVisitAsync(Visit visit);
        Task<IEnumerable<dynamic>> UpdateVisitByTicketAsync(Visit visit);
        Task<IEnumerable<dynamic>> DeleteVisitAsync(long slNo);
        Task<IEnumerable<dynamic>> GetVisitByIdAsync(long slNo);
        Task<IEnumerable<dynamic>> GetAllVisitsAsync();
        Task<IEnumerable<dynamic>> GetVisitsByTicketNoAsync(string ticketNo);
    }
}
