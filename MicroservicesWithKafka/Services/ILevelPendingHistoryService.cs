using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILevelPendingHistoryService
    {
        Task<int> CreateAsync(LevelPendingHistory entity);
        Task<IEnumerable<LevelPendingHistory>> GetAllAsync();
        Task<LevelPendingHistory> GetByIdAsync(int id);
        Task<IEnumerable<LevelPendingHistory>> GetByTicketNoAsync(string ticketNo);
        Task UpdateAsync(LevelPendingHistory entity);
        Task DeleteAsync(int id);
    }
}
