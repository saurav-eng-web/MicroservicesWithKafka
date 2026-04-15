using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LevelPendingHistoryService : ILevelPendingHistoryService
    {
        private readonly ILevelPendingHistoryRepository _repository;

        public LevelPendingHistoryService(ILevelPendingHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task<int> CreateAsync(LevelPendingHistory entity) => _repository.CreateAsync(entity);
        public Task<IEnumerable<LevelPendingHistory>> GetAllAsync() => _repository.GetAllAsync();
        public Task<LevelPendingHistory> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task<IEnumerable<LevelPendingHistory>> GetByTicketNoAsync(string ticketNo) => _repository.GetByTicketNoAsync(ticketNo);
        public Task UpdateAsync(LevelPendingHistory entity) => _repository.UpdateAsync(entity);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
