namespace MicroservicesWithKafka.Services
{
    using MicroservicesWithKafka.Models;
    using MicroservicesWithKafka.Repository;
    using System.Threading.Tasks;

    public class CallTransferService : ICallTransferService
    {
        private readonly ICallTransferRepository _callTransferRepository;

        public CallTransferService(ICallTransferRepository callTransferRepository)
        {
            _callTransferRepository = callTransferRepository;
        }

        public async Task<CallTransferResponse> TransferCallAsync(CallTransferRequest request)
        {
            return await _callTransferRepository.TransferCallAsync(request);
        }
    }
}
