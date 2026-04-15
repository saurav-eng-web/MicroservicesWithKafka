using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class GenerateHappyCodeService : IGenerateHappyCodeService
    {
        private readonly IGenerateHappyCodeRepository _repo;
        private readonly ISMSService _smsService;

        public GenerateHappyCodeService(IGenerateHappyCodeRepository repo, ISMSService smsService)
        {
            _repo = repo;
            _smsService = smsService;
        }

        // Return type changed to match the interface
        public async Task<(IEnumerable<dynamic> Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string userName)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json;

            if (payload is JsonElement element)
                json = JObject.Parse(element.GetRawText());
            else
                json = JObject.FromObject(payload);

            string ticketNo = json["TicketNo"]?.ToString();

            if (string.IsNullOrWhiteSpace(ticketNo))
                throw new ArgumentException("TicketNo is required");

            var request = new
            {
                TicketNo = ticketNo,
                UsrName = userName
            };

            // Call the repository to generate Happy Code
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            // Check if the Happy Code generation was successful
            if (statusCode == 201 && data != null)
            {
                var happyCodeData = data.FirstOrDefault() as IDictionary<string, object>;

                var ticketKey = happyCodeData?["Ticket_no"]?.ToString();
                var message = happyCodeData?["Message"]?.ToString();
                var primaryMobile = happyCodeData?["PMobileNo"]?.ToString();
                var secondaryMobile = happyCodeData?["SMobileNo"]?.ToString();

                //Console.WriteLine("Calling SendSMSAsync...");
                //Console.WriteLine($"Sending SMS with message: {message}");
                //Console.WriteLine($"To Mobile: {secondaryMobile}");
                //Console.WriteLine($"Ticket Key: {ticketKey}");

                //bool smsSentToPrimarya = await _smsService.SendSMSAsync(message, primaryMobile, ticketKey, "Assigned");
                
                //Console.WriteLine($"SendSMSAsync call completed: {smsSentToPrimarya}");
                string smsStatusMessage = "SMS sent successfully.";

                if (!string.IsNullOrWhiteSpace(ticketKey) && !string.IsNullOrWhiteSpace(message))
                {
                    try
                    {
                        // Send SMS to Primary Mobile No
                        bool smsSentToPrimary = false;
                        if (!string.IsNullOrWhiteSpace(primaryMobile))
                        {
                            smsSentToPrimary = await _smsService.SendSMSAsync(message, primaryMobile, ticketKey, "Assigned");
                            if (!smsSentToPrimary)
                            {
                                throw new Exception("Failed to send SMS to Primary Mobile No.");
                            }
                        }

                        // Send SMS to Secondary Mobile No, if different from Primary
                        if (!string.IsNullOrWhiteSpace(secondaryMobile) && primaryMobile != secondaryMobile)
                        {
                            bool smsSentToSecondary = await _smsService.SendSMSAsync(message, secondaryMobile, ticketKey, "Assigned");
                            if (!smsSentToSecondary)
                            {
                                throw new Exception("Failed to send SMS to Secondary Mobile No.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // In case of any error in SMS sending, update the message
                        smsStatusMessage = $"Error while sending SMS: {ex.Message}";
                    }

                    // Update the status message to include the SMS sending result
                    statusMsg = $"{statusMsg} - {smsStatusMessage}";
                }

                // Return Happy Code data along with updated status message
                return (data, statusCode, statusMsg);
            }
            else
            {
                throw new Exception($"Error generating Happy Code: {statusMsg}");
            }
        }
    }
}
