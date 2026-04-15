using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public Task<Customer> CreateAsync(Customer model)
            => _repository.CreateAsync(model);

        public Task<Customer> UpdateAsync(Customer model)
            => _repository.UpdateAsync(model);

        public Task<Customer> DeleteAsync(long id)
            => _repository.DeleteAsync(id);

        public Task<IEnumerable<Customer>> GetAllAsync()
            => _repository.GetAllAsync();

        public Task<Customer?> GetByIdAsync(long id)
            => _repository.GetByIdAsync(id);
    }
}
