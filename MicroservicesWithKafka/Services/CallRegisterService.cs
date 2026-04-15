using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services;
using MicroservicesWithKafka.Services.Kafka;
using MicroservicesWithKafka.Services.Mongo;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public class CallRegisterService : ICallRegisterService
{
    private readonly ICallRegisterRepository _repository;
    private readonly MongoDBService _mongoService;
    private readonly KafkaProducerService _kafkaProducer;
    private readonly IComplaintHistoryService _complaintHistoryService;
    public CallRegisterService(ICallRegisterRepository repository, MongoDBService mongoService, KafkaProducerService kafkaProducer, IComplaintHistoryService complaintHistoryService)
    {
        _repository = repository;
        _mongoService = mongoService;
        _kafkaProducer = kafkaProducer;
        _complaintHistoryService = complaintHistoryService;
    }


    public async Task<string?> CreateComplaintAsync(CallRegisterModel model)
    {

        

        var ticketNo = await _repository.CreateComplaintAsync(model);
        // return await _repository.CreateComplaintAsync(model);

        if (!string.IsNullOrEmpty(ticketNo))
        {
            // Step 2: Fetch the saved record from DB
            var complaint = await GetComplaintByTicketNoAsync(ticketNo);

            // Step 3: (Already handled in GetComplaintByTicketNoAsync)
            // complaint will be published to Kafka inside GetComplaintByTicketNoAsync
        }

        return ticketNo;
    }

    // entry point into kafka
    public async Task<CallRegisterModel?> GetComplaintByTicketNoAsync(string ticketNo)
    {
        // Fetch from SQL via repository
        var complaint = await _repository.GetComplaintByTicketNoAsync(ticketNo);

        if(complaint != null)
        {
            // Publish to Kafka
            string message = JsonSerializer.Serialize(complaint);
            await _kafkaProducer.SendMessageAsync("call-register-events", ticketNo, message);

            // Save into MongoDB
           // await _mongoService.SaveComplaintAsync(complaint);
        }

        return complaint;
    }
    public async Task<List<CallRegisterModel>> GetComplaintsByStatusAsync(string? complaintStatus, string? complaintSubStatus, int? role, int? bid)
    {
        // Fetch directly from SQL via repository
        return await _repository.GetComplaintsByStatusAsync(complaintStatus, complaintSubStatus,role,bid);
    }

    // 🔹 New Method for Approve/Update using primitive parameters
    public async Task<bool> ApproveOrUpdateComplaintAsync(string ticketNo,int userId,string action,string? remarks,string? ascCode,
        string? dropdownSelection,string? complaintStatus,string? enteredBy,string? category)
    {
        return await _repository.ApproveOrUpdateComplaintAsync(ticketNo,userId,action,remarks,ascCode,dropdownSelection,complaintStatus,
            enteredBy,category);
    }

    public async Task<List<CallRegisterModel>> UpdateComplaintAsync(string ticketNo, CallRegisterModel updatedModel)
    {
        // 1️⃣ Update SQL
        var updatedComplaints = await _repository.UpdateComplaintAsync(ticketNo, updatedModel);

        if (updatedComplaints == null || updatedComplaints.Count == 0)
            return new List<CallRegisterModel>();

        // 2️⃣ Pick the first complaint (single object)
        var complaintToSend = updatedComplaints.First();

        // 3️⃣ Serialize the single object for Kafka
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // optional: matches JSON style
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        string message = JsonSerializer.Serialize(complaintToSend, options);
        await _kafkaProducer.SendMessageAsync("update-call-events",ticketNo, message);

        var complaintHistory = new ComplaintHistory
        {
            TicketNo = complaintToSend.TICKET_NO,
            ComplaintStatus = complaintToSend.COMPLAINT_STATUS,
            ComplaintSubStatus = complaintToSend.COMPLAINT_SUBSTATUS+" "+ complaintToSend.dlcity,
            EnteredBy = complaintToSend.CallTransferBY,
            EnteredByID = long.TryParse(complaintToSend.Dealer_ID?.Trim(), out var id)? id: null,
            Tdate = DateTime.UtcNow
        };

        // Send to the "call-history" Kafka topic using ComplaintHistoryService
        await _complaintHistoryService.AddComplaintAsync(complaintHistory);

        // 5️⃣ Return the updated complaints list to keep interface compatible
        return updatedComplaints;

        // 4️⃣ Optional: Save to MongoDB
        // await _mongoService.UpsertCallAsync(ticketNo, complaintToSend);

        // 5️⃣ Return list to keep interface compatible
        return updatedComplaints;

    }

   

}

