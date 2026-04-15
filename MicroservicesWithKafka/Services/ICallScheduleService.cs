using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface ICallScheduleService
    {
        Task<CallSchedule> CreateAsync(CallSchedule model);
        Task<CallSchedule> UpdateAsync(CallSchedule model);
        Task<CallSchedule> DeleteAsync(long id);
        Task<IEnumerable<CallSchedule>> GetAllAsync();
        Task<CallSchedule?> GetByIdAsync(long id);
        Task<CallSchedule?> GetByticketnoAsync(string ticketno);
    }
}
