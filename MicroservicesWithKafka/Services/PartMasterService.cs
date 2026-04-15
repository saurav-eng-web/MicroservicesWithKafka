using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class PartMasterService : IPartMasterService
    {
        private readonly IPartMasterRepository _partMasterRepository;

        public PartMasterService(IPartMasterRepository partMasterRepository)
        {
            _partMasterRepository = partMasterRepository;
        }

        public Task<PartMaster> CreatePartAsync(PartMaster part)
        {
            return _partMasterRepository.CreatePartAsync(part);
        }

        public Task<bool> UpdatePartAsync(PartMaster part)
        {
            return _partMasterRepository.UpdatePartAsync(part);
        }

        public Task<bool> DeletePartAsync(decimal partId)
        {
            return _partMasterRepository.DeletePartAsync(partId);
        }

        public Task<PartMaster?> GetPartByIdAsync(decimal partId)
        {
            return _partMasterRepository.GetPartByIdAsync(partId);
        }

        public Task<List<PartMaster>> GetAllPartsAsync()
        {
            return _partMasterRepository.GetAllPartsAsync();
        }
    }
}
