using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class SymptomMasterService : ISymptomMasterService
    {
        private readonly ISymptomMasterRepository _repository;

        public SymptomMasterService(ISymptomMasterRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<SymptomMaster>> GetAllAsync() => _repository.GetAllAsync();

        public Task<SymptomMaster?> GetByIdAsync(long id) => _repository.GetByIdAsync(id);

        public Task<long> AddAsync(SymptomMaster symptom) => _repository.AddAsync(symptom);

        public Task<long> UpdateAsync(SymptomMaster symptom) => _repository.UpdateAsync(symptom);

        public Task<long> DeleteAsync(long id) => _repository.DeleteAsync(id);

        // ✅ New Method
        public Task<IEnumerable<SymptomMaster>> GetByProductIdAsync(long productId) =>
            _repository.GetByProductIdAsync(productId);
    }
}
