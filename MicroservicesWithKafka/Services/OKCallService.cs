using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class OKCallService : IOKCallService
    {
        private readonly IOKCallRepository _repo;
        private readonly ICallRegisterService _callRegisterService;
        // Added by Saurav on dt: 21-Nov-2026 - SMS service, used to send the "Call OK" SMS (ported from the old WebForms CRM)
        private readonly ISMSService _smsService;

        public OKCallService(IOKCallRepository repo, ICallRegisterService callRegisterService, ISMSService smsService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
            _smsService = smsService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element ? JObject.Parse(element.GetRawText()): JObject.FromObject(payload);

            // Accept camelCase & PascalCase
            string type = json["type"]?.ToString() ?? json["Type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None), // FULL JSON
                sf_id = Convert.ToInt32(sfId)              // FROM JWT
            };

            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            int count = data?.Count() ?? 0;

            if ((statusCode == 200 || statusCode == 201) && count > 0 && type.Equals("okcheck", StringComparison.OrdinalIgnoreCase))
            {
                var updated = await UpdateComplaintsFromPOAsync(data);

                // Added by Saurav on dt: 21-Nov-2026 - "Call OK" SMS (ported from the old WebForms JobsheetUpdate page, source "Call OK")
                await SendCallOkSmsAsync(data, updated);
            }


            return (data, statusCode, statusMsg);
        }

        // Modified by Saurav on dt: 21-Nov-2026 - earlier this method returned only Task. It now returns the updated complaints
        // (needed to get the customer mobile for the "Call OK" SMS). The update logic itself is unchanged.
        private async Task<List<CallRegisterModel>> UpdateComplaintsFromPOAsync(IEnumerable<dynamic> data)
        {
            var updatedComplaints = new List<CallRegisterModel>();

            foreach (var item in data)
            {
                if (item is not IDictionary<string, object> row)
                    continue;

                if (!row.ContainsKey("ticket_no"))
                    continue;

                var model = new CallRegisterModel
                {
                    TICKET_NO = row["ticket_no"]?.ToString(),
                    COMPLAINT_STATUS = row.ContainsKey("COMPLAINT_STATUS") ? row["COMPLAINT_STATUS"]?.ToString() : null,
                    COMPLAINT_SUBSTATUS = row.ContainsKey("Complaint_SubStatus") ? row["Complaint_SubStatus"]?.ToString() : null,
                    dlcity = row.ContainsKey("dlcity") ? row["dlcity"]?.ToString() : null,
                    CallTransferBY = row.ContainsKey("EnterBy") ? row["EnterBy"]?.ToString() : null
                };

                if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
                {
                    var updated = await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO, model);
                    if (updated != null)
                        updatedComplaints.AddRange(updated);
                }
            }

            return updatedComplaints;
        }

        // Added by Saurav on dt: 21-Nov-2026 - Old CRM (JobsheetUpdate.aspx.vb) sent this SMS with source "Call OK" when the call was closed as OK.
        // No stored procedure / DB change needed: mobile comes from UpdateComplaintAsync, the SMS is logged through the existing SMSService.
        private async Task SendCallOkSmsAsync(IEnumerable<dynamic> data, List<CallRegisterModel> updatedComplaints)
        {
            foreach (var item in data)
            {
                if (item is not IDictionary<string, object> row)
                    continue;

                var status = row.ContainsKey("COMPLAINT_STATUS") ? row["COMPLAINT_STATUS"]?.ToString() : null;
                var subStatus = row.ContainsKey("Complaint_SubStatus") ? row["Complaint_SubStatus"]?.ToString() : null;

                // same condition the frontend (OK.js) uses for a successful OK closure
                if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(subStatus, "OK", StringComparison.OrdinalIgnoreCase))
                    continue;

                var ticketNo = row.ContainsKey("ticket_no") ? row["ticket_no"]?.ToString() : null;
                var mobile = updatedComplaints.FirstOrDefault(c => c.TICKET_NO == ticketNo)?.MOBILE;

                if (string.IsNullOrWhiteSpace(ticketNo) || string.IsNullOrWhiteSpace(mobile))
                    continue;

                var message = $"Dear Customer,Your request {ticketNo} has been resolved. Thanks for your association with Sansui. For further assistance, please call 18605003555";

                try
                {
                    await _smsService.SendSMSAsync(message, mobile, ticketNo, "Call OK");
                }
                catch
                {
                    // An SMS failure must never fail the call closure.
                }
            }
        }
    }
}
