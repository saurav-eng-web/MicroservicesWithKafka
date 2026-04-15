using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class WarrantyMasterService : IWarrantyMasterService
    {
        private readonly IWarrantyMasterRepository _repository;

        public WarrantyMasterService(IWarrantyMasterRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<WarrantyMaster>> GetAllAsync() => _repository.GetAllAsync();
        public Task<WarrantyMaster?> GetByIdAsync(long id) => _repository.GetByIdAsync(id);
        public Task<long> AddAsync(WarrantyMaster warranty) => _repository.AddAsync(warranty);
        public Task<long> UpdateAsync(WarrantyMaster warranty) => _repository.UpdateAsync(warranty);
        public Task<long> DeleteAsync(long id) => _repository.DeleteAsync(id);
    }
}
