using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class SMSService : ISMSService
    {
        private readonly IRssoftechSMSRepository _smsRepository;
        private readonly HttpClient _httpClient;

        public SMSService(IRssoftechSMSRepository smsRepository, IHttpClientFactory httpClientFactory)
        {
            _smsRepository = smsRepository;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<bool> SendSMSAsync(string message, string mobile, string ticketKey, string source)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(mobile))
                return false;

            // Replace placeholder {ticketKey} in the template message
            var finalMessage = message.Replace("{ticketKey}", ticketKey);

            //string url = $"http://prpsms.co.in/API/SendMsg.aspx?uname=20181973&pass=dx96fJYw&send=GICARE&dest={mobile}&msg={Uri.EscapeDataString(finalMessage)}"; //Commented by Saurav For Changes API URL on dt: 03-Nov-2026
            string url = $"http://api.bulksmsadmin.com/BulkSMSapi/keyApiSendSMS/SendMsg?uname=20181973&pass=dx96fJYw&send=GICARE&dest={mobile}&msg={Uri.EscapeDataString(finalMessage)}"; //Added by Saurav For Changes API URL on dt: 03-Nov-2026

            string responseText = "";
            bool sent = false;

            try
            {
                var response = await _httpClient.PostAsync(url, null);
                responseText = await response.Content.ReadAsStringAsync();
                sent = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                responseText = $"Error: {ex.Message}";
            }

            var smsRecord = new RssoftechSMS
            {
                Source = source,
                Mobile = mobile,
                Message = finalMessage,
                TicketKey = ticketKey,
                REF = responseText,
                Sent = sent,
                SendDateTime = DateTime.Now
            };

            return await _smsRepository.InsertSentSMSAsync(smsRecord);
        }
    }
}
