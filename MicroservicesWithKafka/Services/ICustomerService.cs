using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface ICustomerService
    {
        Task<Customer> CreateAsync(Customer model);
        Task<Customer> UpdateAsync(Customer model);
        Task<Customer> DeleteAsync(long id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(long id);
    }
}
