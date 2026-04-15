using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class GasChargingService : IGasChargingService
    {
        private readonly IGasChargingRepository _repo;
        private readonly ICallRegisterService _callRegisterService;

        public GasChargingService(IGasChargingRepository repo, ICallRegisterService callRegisterService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
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

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None), // FULL JSON
                sf_id = Convert.ToInt32(sfId)              // FROM JWT
            };

            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);
            int count = data?.Count() ?? 0;

            // NEW: call CallRegisterService
            if ((statusCode == 200 || statusCode == 201) && count > 0 && type.Equals("GCRequest", StringComparison.OrdinalIgnoreCase))
            {
                await UpdateComplaintsFromPOAsync(data);
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

    }
}
