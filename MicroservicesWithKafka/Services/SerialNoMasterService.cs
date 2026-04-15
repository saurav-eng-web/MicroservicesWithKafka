using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class SerialNoMasterService : ISerialNoMasterService
    {
        private readonly ISerialNoMasterRepository _repository;

        public SerialNoMasterService(ISerialNoMasterRepository repository)
        {
            _repository = repository;
        }

        public async Task<SerialNoMaster> CreateSerialNoAsync(SerialNoMaster serial)
        {
            return await _repository.CreateAsync(serial); // ✅ Corrected
        }

        public async Task<bool> UpdateSerialNoAsync(SerialNoMaster serial)
        {
            return await _repository.UpdateAsync(serial); // ✅ Corrected
        }

        public async Task<bool> DeleteSerialNoAsync(decimal id)
        {
            return await _repository.DeleteAsync(id); // ✅ Corrected
        }

        public async Task<SerialNoMaster?> GetSerialNoByIdAsync(decimal id)
        {
            return await _repository.GetByIdAsync(id); // ✅ Corrected
        }

        public async Task<List<SerialNoMaster>> GetAllSerialNosAsync()
        {
            return await _repository.GetAllAsync(); // ✅ Corrected
        }
        public Task<SerialNoMaster?> GetBySerialNumberAsync(string serialNo)
        {
            return _repository.GetBySerialNumberAsync(serialNo);
        }
    }
}
