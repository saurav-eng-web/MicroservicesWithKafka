using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface IReminderService
    {
        Task<long> CreateReminderAsync(Reminder model);
        Task<int> UpdateReminderAsync(Reminder model);
        Task<int> DeleteReminderAsync(long id);
        Task<Reminder?> GetReminderByIdAsync(long id);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();

        Task<IEnumerable<Reminder>> GetReminderByTicketAsync(string ticketNo);



    }
}
