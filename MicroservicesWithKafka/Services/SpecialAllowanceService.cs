using Azure.Core;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services;
using MicroservicesWithKafka.Services.Kafka;
using MicroservicesWithKafka.Services.Mongo;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class SpecialAllowanceService : ISpecialAllowanceService
    {
        private readonly ISpecialAllowanceRepository _repo;
        private readonly MongoDBService _mongoService;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly IComplaintHistoryService _complaintHistoryService;

        public SpecialAllowanceService(ISpecialAllowanceRepository repo, MongoDBService mongoService, KafkaProducerService kafkaProducer, IComplaintHistoryService complaintHistoryService)
        {
            _repo = repo;
            _mongoService = mongoService;
            _kafkaProducer = kafkaProducer;
            _complaintHistoryService = complaintHistoryService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object request)
        {
            // 1️⃣ Call repository ONCE
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            // 2️⃣ Only on SUCCESS + Action == INSERT → Kafka
            if (statusCode == 200 && request is SpecialAllowanceRequest model && string.Equals(model.Action, "INSERT", StringComparison.OrdinalIgnoreCase))
            {
                var complaintHistory = new ComplaintHistory
                {
                    TicketNo = model.Ticket_No,
                    ComplaintStatus = "Type: " + model.SpecialApprovaltype + " | Amount requested: " + model.NSPRequestAmount,
                    ComplaintSubStatus = "Remarks: " + model.NSPRemarks,
                    EnteredBy = model.NSPRequestBy,
                    EnteredByID = 0,
                    Tdate = DateTime.UtcNow
                };
                await _complaintHistoryService.AddComplaintAsync(complaintHistory);
            }
            if (statusCode == 200 && request is SpecialAllowanceRequest model1 && string.Equals(model1.Action, "UPDATERSM", StringComparison.OrdinalIgnoreCase))
                {
                  var complaintHistoryrsm = new ComplaintHistory
                    {
                        TicketNo = model1.Ticket_No,
                        ComplaintStatus = "Status: " + model1.sa_status + " | Amount requested: " + model1.RSMApproveAmount,
                        ComplaintSubStatus = "Remarks: " + model1.RSMRemarks,
                        EnteredBy = model1.NSPRequestBy,
                        EnteredByID = 0,
                        Tdate = DateTime.UtcNow
                    };
                 await _complaintHistoryService.AddComplaintAsync(complaintHistoryrsm);
            }

            // 3️⃣ Return original DB result
            return (data, statusCode, statusMsg);
        }


    }
}
