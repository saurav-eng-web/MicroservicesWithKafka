using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class DepartmentMasterService : IDepartmentMasterService
    {
        private readonly IDepartmentMasterRepository _repository;

        public DepartmentMasterService(IDepartmentMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DepartmentMaster>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<DepartmentMaster?> GetByIdAsync(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(DepartmentMaster model)
        {
            return await _repository.CreateAsync(model);
        }

        public async Task<bool> UpdateAsync(DepartmentMaster model)
        {
            return await _repository.UpdateAsync(model);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
