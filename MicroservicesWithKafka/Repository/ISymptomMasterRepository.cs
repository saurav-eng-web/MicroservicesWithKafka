using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ISymptomMasterRepository
    {
        Task<IEnumerable<SymptomMaster>> GetAllAsync();
        Task<SymptomMaster?> GetByIdAsync(long id);
        Task<long> AddAsync(SymptomMaster symptom);
        Task<long> UpdateAsync(SymptomMaster symptom);
        Task<long> DeleteAsync(long id);

        Task<IEnumerable<SymptomMaster>> GetByProductIdAsync(long productId);
    }
}
