namespace MicroservicesWithKafka.Repository
{
    using System.Threading.Tasks;
    using MicroservicesWithKafka.Models;

    public interface ICallTransferRepository
    {
        Task<CallTransferResponse> TransferCallAsync(CallTransferRequest request);
    }
}
