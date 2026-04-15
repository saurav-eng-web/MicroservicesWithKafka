using System.Collections.Generic;
using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _repository;

        public ReminderService(IReminderRepository repository)
        {
            _repository = repository;
        }

        public async Task<long> CreateReminderAsync(Reminder model)
        {
            return await _repository.InsertAsync(model);
        }

        public async Task<int> UpdateReminderAsync(Reminder model)
        {
            return await _repository.UpdateAsync(model);
        }

        public async Task<int> DeleteReminderAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<Reminder?> GetReminderByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Reminder>> GetReminderByTicketAsync(string ticketNo)
        {
            return await _repository.GetByTicketAsync(ticketNo);
        }


    }
}
