using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class CallScheduleService : ICallScheduleService
    {
        private readonly ICallScheduleRepository _repository;

        public CallScheduleService(ICallScheduleRepository repository)
        {
            _repository = repository;
        }

        public Task<CallSchedule> CreateAsync(CallSchedule model)
            => _repository.CreateAsync(model);

        public Task<CallSchedule> UpdateAsync(CallSchedule model)
            => _repository.UpdateAsync(model);

        public Task<CallSchedule> DeleteAsync(long id)
            => _repository.DeleteAsync(id);

        public Task<IEnumerable<CallSchedule>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<CallSchedule?> GetByIdAsync(long id)
            => _repository.GetByIdAsync(id);
        public Task<CallSchedule?> GetByticketnoAsync(string ticketno)
           => _repository.GetByticketnoAsync(ticketno);
    }
}
