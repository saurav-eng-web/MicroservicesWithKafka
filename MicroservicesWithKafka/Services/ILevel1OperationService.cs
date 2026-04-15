using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface ILevel1OperationService
    {
        Task<Level1OperationResponse> Level1OperationAsync(Level1OperationRequest request);
    }
}
