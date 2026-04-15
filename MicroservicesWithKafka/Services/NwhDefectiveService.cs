using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repositories;

namespace MicroservicesWithKafka.Services
{
    public class NwhDefectiveService : INwhDefectiveService
    {
        private readonly INwhDefectiveRepository _repository;

        public NwhDefectiveService(INwhDefectiveRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<NwhDefectiveResponse>> ManageNwhDefectiveAsync(NwhDefectiveRequest request)
        {
            return _repository.ManageNwhDefectiveAsync(request);
        }
    }
}
