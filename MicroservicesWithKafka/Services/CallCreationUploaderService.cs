using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CallCreationUploaderService : ICallCreationUploaderService
    {
        private readonly ICallCreationUploaderRepository _repository;

        public CallCreationUploaderService(ICallCreationUploaderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<int>> CreateBulkAsync(List<CallCreationUploader> models)
        {
            return await _repository.CreateBulkAsync(models);
        }



        public Task<string> UpdateAsync(CallCreationUploader model) => _repository.UpdateAsync(model);

        public Task<string> DeleteAsync(int id) => _repository.DeleteAsync(id);

        public Task<IEnumerable<CallCreationUploader>> GetAllAsync() => _repository.GetAllAsync();

        public Task<CallCreationUploader?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    }
}
