using MicroservicesWithKafka.Repository;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class JobnOtherDetailNewService : IJobnOtherDetailNewService
    {
        private readonly IJobnOtherDetailNewRepository _repository;
        private readonly ILogger<JobnOtherDetailNewService> _logger;

        public JobnOtherDetailNewService(
            IJobnOtherDetailNewRepository repository,
            ILogger<JobnOtherDetailNewService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)>
            GetAsync(string ticketNo, string type)
        {
            _logger.LogInformation(
                "Fetching Job/Other details. TicketNo: {TicketNo}, Type: {Type}",
                ticketNo, type);

            return await _repository.GetAsync(ticketNo, type);
        }
    }
}
