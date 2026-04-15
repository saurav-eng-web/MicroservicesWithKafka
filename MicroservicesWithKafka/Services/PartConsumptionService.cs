using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class PartConsumptionService : IPartConsumptionService
    {
        private readonly IPartConsumptionRepository _repository;

        public PartConsumptionService(IPartConsumptionRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<PartConsumption>> GetByTicketNoAsync(string ticketNo) =>
            _repository.GetByTicketNoAsync(ticketNo);

        public Task<PartConsumption?> GetByIdAsync(int id) =>
            _repository.GetByIdAsync(id);

        public Task<int> InsertAsync(PartConsumption partConsumption) =>
            _repository.InsertAsync(partConsumption);

        public Task UpdateAsync(PartConsumption partConsumption) =>
            _repository.UpdateAsync(partConsumption);

        public Task DeleteAsync(int id) =>
            _repository.DeleteAsync(id);
    }
}
