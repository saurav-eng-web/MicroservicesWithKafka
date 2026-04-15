using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface IPartMasterRepository
    {
        Task<PartMaster> CreatePartAsync(PartMaster part);
        Task<bool> UpdatePartAsync(PartMaster part);
        Task<bool> DeletePartAsync(decimal partId);
        Task<PartMaster?> GetPartByIdAsync(decimal partId);
        Task<List<PartMaster>> GetAllPartsAsync();
    }
}
