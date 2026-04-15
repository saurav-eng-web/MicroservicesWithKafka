using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class RequestCancellationService : IRequestCancellationService
    {
        private readonly IRequestCancellationRepository _repository;

        public RequestCancellationService(IRequestCancellationRepository repository)
        {
            _repository = repository;
        }

        public Task<RequestCancellation> CreateAsync(RequestCancellation model)
            => _repository.CreateAsync(model);

        public Task<RequestCancellation> UpdateAsync(RequestCancellation model)
            => _repository.UpdateAsync(model);

        public Task<RequestCancellation> DeleteAsync(long id)
            => _repository.DeleteAsync(id);

        public Task<IEnumerable<RequestCancellation>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<RequestCancellation?> GetByIdAsync(long id)
            => _repository.GetByIdAsync(id);
    }
}
