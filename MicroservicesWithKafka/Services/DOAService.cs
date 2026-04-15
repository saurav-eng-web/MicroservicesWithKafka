using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class DOAService : IDOAService
    {
        private readonly IDOARepository _repo;
        private readonly ICallRegisterService _callRegisterService;

        public DOAService(IDOARepository repo, ICallRegisterService callRegisterService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json;

            if (payload is JsonElement element)
                json = JObject.Parse(element.GetRawText());
            else
                json = JObject.FromObject(payload);

            string type = json["type"]?.ToString();
            string ticketNo = json["TicketNo"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                type = type,
                ticket_no = ticketNo,
                sf_id = Convert.ToInt32(sfId),   //FROM TOKEN
                payload = json.ToString(Formatting.None)
            };
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);

            int count = data?.Count() ?? 0;

            // NEW: call CallRegisterService
            if ((statusCode == 200 || statusCode == 201) && count > 0 && type.Equals("DOAReceive", StringComparison.OrdinalIgnoreCase))
            {
                await UpdateComplaintsFromPOAsync(data);
            }
            return (data, statusCode, statusMsg);
            //return await _repo.ExecuteAsync(request);
        }

        private async Task UpdateComplaintsFromPOAsync(IEnumerable<dynamic> data)
        {
            var validRow = data
                .FirstOrDefault(x =>
                {
                    var dict = x as IDictionary<string, object>;
                    return dict != null && dict.ContainsKey("Ticket_no");
                });

            if (validRow == null)
                return;

            var row = (IDictionary<string, object>)validRow;

            var model = new CallRegisterModel
            {
                TICKET_NO = row["Ticket_no"]?.ToString(),
                COMPLAINT_STATUS = row["Compaint_status"]?.ToString(),
                COMPLAINT_SUBSTATUS = row["Compaint_Substatus"]?.ToString(),
                dlcity = row["dlcity"]?.ToString(),
                CallTransferBY = row["Enterby"]?.ToString()
            };

            if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
            {
                await _callRegisterService.UpdateComplaintAsync(
                    model.TICKET_NO,
                    model
                );
            }
        }


    }
}
