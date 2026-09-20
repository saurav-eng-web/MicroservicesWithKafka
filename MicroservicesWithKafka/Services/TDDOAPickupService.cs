using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class TDDOAPickupService : ITDDOAPickupService
    {
        private readonly ITDDOAPickupRepository _repo;

        public TDDOAPickupService(ITDDOAPickupRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string userId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            // ACCEPT BOTH camelCase & PascalCase
            string action = json["action"]?.ToString() ?? json["Action"]?.ToString();

            string serialNumber = json["SerialNumber"]?.ToString() ?? json["serialNumber"]?.ToString();

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("action is required");

            // SP reads the payload with JSON_VALUE, whose paths are CASE-SENSITIVE.
            // Normalise to the exact keys the SP looks for: $.action, $.SerialNumber, $.userid
            json["action"] = action;

            if (!string.IsNullOrWhiteSpace(serialNumber))
                json["SerialNumber"] = serialNumber;

            json["userid"] = userId;

            var request = new
            {
                payload = json.ToString(Formatting.None) // FULL JSON
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
