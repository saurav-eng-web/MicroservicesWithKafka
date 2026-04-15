using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ISymptomMasterService
    {
        Task<IEnumerable<SymptomMaster>> GetAllAsync();
        Task<SymptomMaster?> GetByIdAsync(long id);
        Task<long> AddAsync(SymptomMaster symptom);
        Task<long> UpdateAsync(SymptomMaster symptom);
        Task<long> DeleteAsync(long id);

        // ✅ New Method
        Task<IEnumerable<SymptomMaster>> GetByProductIdAsync(long productId);
    }
}
