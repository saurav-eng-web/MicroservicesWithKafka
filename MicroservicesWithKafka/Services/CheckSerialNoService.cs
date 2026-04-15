using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class CheckSerialNoService : ICheckSerialNoService
    {
        private readonly ICheckSerialNoRepository _repo;
        private readonly ICallRegisterService _callRegisterService;

        public CheckSerialNoService(ICheckSerialNoRepository repo, ICallRegisterService callRegisterService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
           ProcessAsync(object payload)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            // ACCEPT BOTH camelCase & PascalCase
            string serialNo =json["SerialNo"]?.ToString() ??json["serialNo"]?.ToString();

            string ticketNo =json["TicketNo"]?.ToString() ??json["ticketNo"]?.ToString();

            string type =json["Type"]?.ToString() ?? json["type"]?.ToString();

            int? pincode = json["Pincode"]?.ToObject<int?>() ?? json["pincode"]?.ToObject<int?>();

            if (string.IsNullOrWhiteSpace(serialNo))
                throw new ArgumentException("SerialNo is required");

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Type is required");

            var request = new
            {
                serialno = serialNo,
                Ticketno = ticketNo,
                pincode = pincode,
                type = type
            };

            var (data, statusCode, statusMsg) =
                await _repo.ExecuteAsync(request);

            // Only for WIPProcess
            if ((statusCode == 200 || statusCode == 201) && type.Equals("WIPProcess", StringComparison.OrdinalIgnoreCase))
            {
                await UpdateComplaintsFromPOAsync(data);
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
                    await _callRegisterService
                        .UpdateComplaintAsync(model.TICKET_NO, model);
                }
            }
        }
    }
}
