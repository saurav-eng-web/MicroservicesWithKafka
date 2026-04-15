using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface IReminderRepository
    {
        Task<long> InsertAsync(Reminder model);
        Task<int> UpdateAsync(Reminder model);
        Task<int> DeleteAsync(long id);
        Task<Reminder?> GetByIdAsync(long id);
        Task<IEnumerable<Reminder>> GetAllAsync();

        Task<IEnumerable<Reminder>> GetByTicketAsync(string ticketNo);

    }
}

