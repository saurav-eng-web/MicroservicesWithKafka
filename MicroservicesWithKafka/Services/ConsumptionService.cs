using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class ConsumptionService : IConsumptionService
    {
        private readonly IConsumptionRepository _repo;
        private readonly ICallRegisterService _callRegisterService;
        private readonly ISMSService _smsService;
        public ConsumptionService(IConsumptionRepository repo, ICallRegisterService callRegisterService, ISMSService smsService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
            _smsService = smsService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element ? JObject.Parse(element.GetRawText()): JObject.FromObject(payload);

            // Read & normalize type
            string type = json["type"]?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            // Normalize ticket_no for SP
            if (json["ticket_no"] == null && json["TicketNo"] != null) json["ticket_no"] = json["TicketNo"];

            var request = new
            {
                Payload = json.ToString(Formatting.None),
                SF_id = Convert.ToInt32(sfId)
            };

            // Execute SP ONCE
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            int count = data?.Count() ?? 0;

            // CALL ONLY FOR PNAPartConsume
            if ((statusCode == 200 || statusCode == 201) && count > 0 && (type.Equals("PNAPartConsume", StringComparison.OrdinalIgnoreCase) || type.Equals("MSLPartConsume", StringComparison.OrdinalIgnoreCase) || type.Equals("DOAConsume", StringComparison.OrdinalIgnoreCase)))
            {
                await UpdateComplaintsFromPOAsync(data);
            }


            // Send SMS ONLY for DOAConsume
            if ((statusCode == 200 || statusCode == 201) && count > 0 &&  type.Equals("DOAConsume", StringComparison.OrdinalIgnoreCase))
            {
                statusMsg = await SendDoaConsumeSmsAsync(data, statusMsg);
            }

            return (data, statusCode, statusMsg);          
           
        }


        private async Task UpdateComplaintsFromPOAsync(IEnumerable<dynamic> data)
        {
            foreach (var item in data)
            {
                if (item is not IDictionary<string, object> row)
                    continue;

                if (!row.ContainsKey("Ticket_no"))
                    continue;

                var model = new CallRegisterModel
                {
                    TICKET_NO = row["Ticket_no"]?.ToString(),
                    COMPLAINT_STATUS = row.ContainsKey("Complaint_status")? row["Complaint_status"]?.ToString(): null,
                    COMPLAINT_SUBSTATUS = row.ContainsKey("Complaint_Substatus")? row["Complaint_Substatus"]?.ToString(): null,
                    dlcity = row.ContainsKey("dlcity")? row["dlcity"]?.ToString(): null,
                    CallTransferBY = row.ContainsKey("EnterBy")? row["EnterBy"]?.ToString(): null
                };

                if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
                {
                    await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO,model);
                }
            }
        }

        private async Task<string> SendDoaConsumeSmsAsync(IEnumerable<dynamic> data,string statusMsg)
        {
            if (data == null)
                return statusMsg;

            var doaData = data.FirstOrDefault() as IDictionary<string, object>;
            if (doaData == null)
                return statusMsg;

            var ticketKey = doaData?["Ticket_no"]?.ToString();
            var message = doaData?["Message"]?.ToString();
            var primaryMobile = doaData?["PMobileNo"]?.ToString();
            var secondaryMobile = doaData?["SMobileNo"]?.ToString();

          
            if (string.IsNullOrWhiteSpace(ticketKey) || string.IsNullOrWhiteSpace(message))
                return statusMsg;

            string smsStatusMessage = "SMS sent successfully.";

            try
            {
                // Primary Mobile
                if (!string.IsNullOrWhiteSpace(primaryMobile))
                {
                    bool sentPrimary = await _smsService.SendSMSAsync(message,primaryMobile,ticketKey,"DOAConsume");

                    if (!sentPrimary)
                        throw new Exception("Failed to send SMS to Primary Mobile No.");
                }

                // Secondary Mobile (if different)
                if (!string.IsNullOrWhiteSpace(secondaryMobile) &&  secondaryMobile != primaryMobile)
                {
                    bool sentSecondary = await _smsService.SendSMSAsync(message,secondaryMobile,ticketKey,"DOAConsume");

                    if (!sentSecondary)
                        throw new Exception("Failed to send SMS to Secondary Mobile No.");
                }
            }
            catch (Exception ex)
            {
                smsStatusMessage = $"Error while sending SMS: {ex.Message}";
            }

            return $"{statusMsg} - {smsStatusMessage}";
        }



    }
}
