using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class ClaimProcessServiceV1 : IClaimProcessServiceV1
    {
        private readonly IClaimProcessRepositoryV1 _repo;
        private readonly IComplaintHistoryService _complaintHistoryService;

        public ClaimProcessServiceV1(
            IClaimProcessRepositoryV1 repo,
            IComplaintHistoryService complaintHistoryService)
        {
            _repo = repo;
            _complaintHistoryService = complaintHistoryService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, int userId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement el
                ? JObject.Parse(el.GetRawText())
                : JObject.FromObject(payload);

            string type = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None),
                userid = userId
            };

            var (data, code, msg) = await _repo.ExecuteAsync(request);

            // 🔥 Insert complaint history ONLY when SP returns dataset
            if (code == 200 && data.Any() &&
                type.Equals("ClaimDetailAction", StringComparison.OrdinalIgnoreCase))
            {
                await InsertComplaintHistoryAsync(data, json, msg);
            }

            return (data, code, msg);
        }

        private async Task InsertComplaintHistoryAsync(
            IEnumerable<dynamic> data,
            JObject requestJson,
            string statusMsg)
        {
            string fallbackTicket = requestJson["ticket_no"]?.ToString();

            foreach (var item in data)
            {
                var row = item as IDictionary<string, object>;

                var history = new ComplaintHistory
                {
                    TicketNo = row?["ticket_no"]?.ToString() ?? fallbackTicket,
                    ComplaintStatus = row?["Complaint_status"]?.ToString() ?? "Open",
                    ComplaintSubStatus = row != null
                        ? $"{row["Complaint_SubStatus"]} {row["dlcity"]}"
                        : statusMsg,
                    EnteredBy = row?["EnterBy"]?.ToString(),
                    EnteredByID = null,
                    Tdate = DateTime.UtcNow
                };

                await _complaintHistoryService.AddComplaintAsync(history);
            }
        }
    }
}
