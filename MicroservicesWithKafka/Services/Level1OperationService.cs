using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class Level1OperationService : ILevel1OperationService
    {
        private readonly ILevel1OperationRepository _repo;

        public Level1OperationService(ILevel1OperationRepository repo)
        {
            _repo = repo;
        }

        public async Task<Level1OperationResponse> Level1OperationAsync(Level1OperationRequest request)
        {
            return await _repo.Level1OperationAsync(request);
        }
    }
}
