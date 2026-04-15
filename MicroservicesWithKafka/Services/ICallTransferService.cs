namespace MicroservicesWithKafka.Services
{
    using MicroservicesWithKafka.Models;
    using System.Threading.Tasks;

    public interface ICallTransferService
    {
        Task<CallTransferResponse> TransferCallAsync(CallTransferRequest request);
    }
}
