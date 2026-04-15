using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class CancellationRequestService : ICancellationRequestService
    {
        private readonly ICancellationRequestRepository _repo;
        private readonly ICallRegisterService _callRegisterService;

        public CancellationRequestService(ICancellationRequestRepository repo, ICallRegisterService callRegisterService)
        {
            _repo = repo;
            _callRegisterService = callRegisterService;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            string type = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None),
                sf_id = Convert.ToInt32(sfId)
            };

            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);
            int count = data?.Count() ?? 0;

            if ((statusCode == 200 || statusCode == 201) && count > 0 && type.Equals("POCancelRequest", StringComparison.OrdinalIgnoreCase) || type.Equals("JobCancelRequest", StringComparison.OrdinalIgnoreCase))
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
                COMPLAINT_STATUS = row.ContainsKey("Compaint_status") ? row["Compaint_status"]?.ToString() : null,
                COMPLAINT_SUBSTATUS = row.ContainsKey("Complaint_SubStatus") ? row["Complaint_SubStatus"]?.ToString() : null,
                dlcity = row.ContainsKey("dlcity") ? row["dlcity"]?.ToString() : null,
                CallTransferBY = row.ContainsKey("EnterBy") ? row["EnterBy"]?.ToString() : null
            };

            if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
            {
                await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO,model);
            }
        }
    }
}
