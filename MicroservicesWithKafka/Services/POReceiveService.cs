using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class POReceiveService : IPOReceiveService
    {
        private readonly IPOReceiveRepository _repo;
        private readonly ICallRegisterService _callRegisterService;

        public POReceiveService(IPOReceiveRepository repo, ICallRegisterService callRegisterService)
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
            string search = json["search"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            // FULL PAYLOAD JSON (for SQL)
            string payloadJson = json.ToString(Formatting.None);

            var request = new
            {
                type = type,
                sf_id = Convert.ToInt32(sfId),
                search = string.IsNullOrWhiteSpace(search) ? null : search,
                payload = payloadJson      // PASSED TO SP
            };

            // Existing DB call
            var (data, statusCode, statusMsg) = await _repo.ExecuteAsync(request);
            int count = data?.Count() ?? 0;
            // NEW: call CallRegisterService
            if ((statusCode == 200 || statusCode == 201) && count > 0 && type.Equals("Recieve", StringComparison.OrdinalIgnoreCase))
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
                    COMPLAINT_STATUS = row.ContainsKey("COMPLAINT_STATUS")? row["COMPLAINT_STATUS"]?.ToString(): null,
                    COMPLAINT_SUBSTATUS = row.ContainsKey("Complaint_SubStatus")? row["Complaint_SubStatus"]?.ToString(): null,
                    dlcity = row.ContainsKey("dl_city")? row["dl_city"]?.ToString(): null,
                    CallTransferBY = row.ContainsKey("Enterby")? row["Enterby"]?.ToString(): null
                };

                if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
                {
                    await _callRegisterService.UpdateComplaintAsync(model.TICKET_NO,model);
                }
            }
        }


        //private async Task UpdateComplaintsFromPOAsync(IEnumerable<dynamic> data)
        //{
        //    var validRow = data
        //        .FirstOrDefault(x =>
        //        {
        //            var dict = x as IDictionary<string, object>;
        //            return dict != null && dict.ContainsKey("Ticket_no");
        //        });

        //    if (validRow == null)
        //        return;

        //    var row = (IDictionary<string, object>)validRow;

        //    var model = new CallRegisterModel
        //    {
        //        TICKET_NO = row["Ticket_no"]?.ToString(),
        //        COMPLAINT_STATUS = row["COMPLAINT_STATUS"]?.ToString(),
        //        COMPLAINT_SUBSTATUS = row["Complaint_SubStatus"]?.ToString(),
        //        dlcity = row["dl_city"]?.ToString(),
        //        CallTransferBY = row["Enterby"]?.ToString()

        //    };

        //    if (!string.IsNullOrWhiteSpace(model.TICKET_NO))
        //    {
        //        await _callRegisterService.UpdateComplaintAsync(
        //            model.TICKET_NO,
        //            model
        //        );
        //    }
        //}

    }
}
