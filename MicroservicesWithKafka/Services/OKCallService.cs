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

        public OKCallService(IOKCallRepository repo, ICallRegisterService callRegisterService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
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
                    await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO, model);
                }
            }
        }
    }
}
