using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services.Kafka;
using MicroservicesWithKafka.Services.Mongo;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class HappycodeJobCloseCheckService
        : IHappycodeJobCloseCheckService
    {
        private readonly IHappycodeJobCloseCheckRepository _repo;
        private readonly IComplaintHistoryService _complaintHistoryService;

        public HappycodeJobCloseCheckService(IHappycodeJobCloseCheckRepository repo, IComplaintHistoryService complaintHistoryService)
        {
            _repo = repo;
            _complaintHistoryService = complaintHistoryService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object payload)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            string ticketNo = json["TicketNo"]?.ToString();
            string pin = json["Pin"]?.ToString();

            if (string.IsNullOrWhiteSpace(ticketNo))
                throw new ArgumentException("TicketNo is required");

            if (string.IsNullOrWhiteSpace(pin))
                throw new ArgumentException("Pin is required");

            var request = new
            {
                ticket_no = ticketNo,
                pin = pin
            };

            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            // ONLY condition → StatusCode == 200
            if (statusCode == 200)
            {
                IDictionary<string, object> row = null;

                // If SP returned data, use it
                if (data != null && data.Any())
                    row = data.First() as IDictionary<string, object>;

                var complaintHistory = new ComplaintHistory
                {
                    TicketNo = row?["Ticket_no"]?.ToString() ?? ticketNo,
                    ComplaintStatus = row?["Complaint_status"]?.ToString() ?? "Open",
                    ComplaintSubStatus = row != null? $"{row["Complaint_Substatus"]} {row["dlcity"]}": statusMsg, // fallback for invalid happy code
                    EnteredBy = row?["EnterBy"]?.ToString(),
                    EnteredByID = null,
                    Tdate = DateTime.UtcNow
                };

                await _complaintHistoryService.AddComplaintAsync(complaintHistory);
            }

            return (data, statusCode, statusMsg);
        }

    }
}
