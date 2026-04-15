using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class WarrantyRegistrationService : IWarrantyRegistrationService
    {
        private readonly IWarrantyRegistrationRepository _repository;

        public WarrantyRegistrationService(IWarrantyRegistrationRepository repository)
        {
            _repository = repository;
        }

        public Task<long> AddAsync(WarrantyRegistration model)
        {
            return _repository.InsertAsync(model);
        }

        public Task<bool> UpdateAsync(WarrantyRegistration model)
        {
            return _repository.UpdateAsync(model);
        }

        public Task<bool> DeleteAsync(long id)
        {
            return _repository.DeleteAsync(id);
        }

        public Task<List<WarrantyRegistration>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<WarrantyRegistration?> GetByIdAsync(long id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<WarrantyRegistration?> GetBySerialNumberAsync(string serialNumber)
        {
            return _repository.GetBySerialNumberAsync(serialNumber);
        }
    }
}
