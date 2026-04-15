using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface ILevel1OperationRepository
    {
        Task<Level1OperationResponse> Level1OperationAsync(Level1OperationRequest request);
    }
}
