using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IPartConsumptionRepository
    {
        Task<IEnumerable<PartConsumption>> GetByTicketNoAsync(string ticketNo);
        Task<PartConsumption?> GetByIdAsync(int id);
        Task<int> InsertAsync(PartConsumption partConsumption);
        Task UpdateAsync(PartConsumption partConsumption);
        Task DeleteAsync(int id);
    }
}
