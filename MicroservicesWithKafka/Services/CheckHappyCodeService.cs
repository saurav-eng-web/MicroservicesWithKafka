using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services.Kafka;
using MicroservicesWithKafka.Services.Mongo;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class CheckHappyCodeService : ICheckHappyCodeService
    {
        private readonly ICheckHappyCodeRepository _repo;
        private readonly MongoDBService _mongoService;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly IComplaintHistoryService _complaintHistoryService;
        private readonly ILogger<CheckHappyCodeService> _logger;

        public CheckHappyCodeService(ICheckHappyCodeRepository repo,MongoDBService mongoService,KafkaProducerService kafkaProducer,IComplaintHistoryService complaintHistoryService,ILogger<CheckHappyCodeService> logger)
        {
            _repo = repo;
            _mongoService = mongoService;
            _kafkaProducer = kafkaProducer;
            _complaintHistoryService = complaintHistoryService;
            _logger = logger;
        }

        public async Task<dynamic> CheckAsync(string TK, string UserName)
        {
            try
            {
                // Call the repository to execute the stored procedure and get the result
                var result = await _repo.ExecuteAsync(TK, UserName);

                // Check if result is not null
                if (result != null)
                {
                    // Map the result to a ComplaintHistory object
                    var complaintHistory = new ComplaintHistory
                    {
                        TicketNo = TK, // Use TK as the TicketNo
                        ComplaintStatus = result.StatusMsg, // Map StatusMsg to ComplaintStatus
                        ComplaintSubStatus = "Happy code sent to customer",//result.primaryno, // Map primaryno to ComplaintSubStatus
                        EnteredBy = UserName, // Use UserName as EnteredBy
                        EnteredByID = null, // Assuming no ID in this context, but you can set it if available
                        Tdate = DateTime.UtcNow // Set current UTC date and time
                    };

                    // Send the complaint history object to the Kafka topic using the Kafka service
                    await _complaintHistoryService.AddComplaintAsync(complaintHistory);
                }

                // Return the result from the repository
                return result;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                _logger.LogError(ex, "An error occurred while processing the Happy Code check.");

                // Return a custom error message in case of failure
                return new
                {
                    StatusMsg = "An error occurred while processing the request.",
                    StatusCode = 500
                };
            }
        }
    }
}
