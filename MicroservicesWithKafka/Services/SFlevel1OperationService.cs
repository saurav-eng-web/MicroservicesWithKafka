using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class SFlevel1OperationService : ISFlevel1OperationService
    {
        private readonly ISFlevel1OperationRepository _repo;
        private readonly ICallRegisterService _callRegisterService;
        private readonly IGenerateHappyCodeService _generateHappyCodeService;
        private readonly ISMSService _smsService;

        public SFlevel1OperationService(ISFlevel1OperationRepository repo,ICallRegisterService callRegisterService,IGenerateHappyCodeService generateHappyCodeService,ISMSService smsService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
            _generateHappyCodeService = generateHappyCodeService;
            _smsService = smsService;
        }


        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json;

            if (payload is JsonElement element)
                json = JObject.Parse(element.GetRawText());
            else
                json = JObject.FromObject(payload);

            string type = json["type"]?.ToString();
            string ticketNo = json["ticket_no"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            if (string.IsNullOrWhiteSpace(ticketNo))
                throw new ArgumentException("ticket_no is required");

            var request = new
            {
                Payload = json.ToString(Formatting.None), // FULL JSON
                sf_id = sfId
            };

            // Existing DB call
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);
            int count = data?.Count() ?? 0;

            // NEW: call CallRegisterService

            if ((statusCode == 200 || statusCode == 201) && count > 0 && (type.Equals("assigned", StringComparison.OrdinalIgnoreCase) || type.Equals("Re-Allot", StringComparison.OrdinalIgnoreCase) || type.Equals("Part Issued To Engg", StringComparison.OrdinalIgnoreCase) || type.Equals("Under Warranty Void", StringComparison.OrdinalIgnoreCase)))
            {
                await UpdateComplaintsFromPOAsync(data);
            }


            // Send SMS ONLY for DOAConsume
            if ((statusCode == 200 || statusCode == 201) && type.Equals("assigned", StringComparison.OrdinalIgnoreCase))
            {
                // After updating complaints, generate the Happy Code and send SMS
                await GenerateHappyCodeAndSendSMSAsync(data);
            }

            //return await _repo.ExecuteAsync(request);
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
                    COMPLAINT_STATUS = row.ContainsKey("Complaint_status") ? row["Complaint_status"]?.ToString() : null,
                    COMPLAINT_SUBSTATUS = row.ContainsKey("Complaint_Substatus") ? row["Complaint_Substatus"]?.ToString() : null,
                    dlcity = row.ContainsKey("dlcity") ? row["dlcity"]?.ToString() : null,
                    CallTransferBY = row.ContainsKey("EnterBy") ? row["EnterBy"]?.ToString() : null
                };

                if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
                {
                    await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO, model);
                }
            }
        }

        private async Task GenerateHappyCodeAndSendSMSAsync(IEnumerable<dynamic> data)
        {
            foreach (var item in data)
            {
                string ticketNo = item?.Ticket_no?.ToString();

                if (string.IsNullOrWhiteSpace(ticketNo))
                    continue;

                try
                {
                    // Generate Happy Code for the ticket
                    var happyCodeResponse = await _generateHappyCodeService.ProcessAsync(new { TicketNo = ticketNo }, "Assigned");

                    //if (happyCodeResponse.Item2 == 201)
                    //{
                    //    var happyCodeData = happyCodeResponse.Item1?.FirstOrDefault();
                    //    var ticketKey = happyCodeData?.Ticket_no?.ToString();
                    //    var message = happyCodeData?.Message?.ToString();
                    //    var mobile = happyCodeData?.GetValue("Primary Mobile No")?.ToString();

                    //    if (!string.IsNullOrWhiteSpace(ticketKey) && !string.IsNullOrWhiteSpace(mobile))
                    //    {
                    //        // Send the SMS
                    //        var smsSent = await _smsService.SendSMSAsync(message, mobile, ticketKey, "Service Request");

                    //        if (!smsSent)
                    //        {
                    //            throw new Exception("Failed to send SMS after generating Happy Code.");
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    // Log any error that occurs during the Happy Code generation or SMS sending process
                    Console.WriteLine($"Error generating Happy Code or sending SMS: {ex.Message}");
                }
            }
        }

    }
}
