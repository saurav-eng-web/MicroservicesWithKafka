using MicroservicesWithKafka.Repository;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class TDDOAPickupService : ITDDOAPickupService
    {
        private readonly ITDDOAPickupRepository _repo;

        // Actions supported by SP_CRUD_DOTDCNDISTPICKUP
        private static readonly string[] AllowedActions =
        {
            "INSERT", "DISTSELECT", "LOGSELECT"
        };

        public TDDOAPickupService(ITDDOAPickupRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string userId)
        {
            if (payload == null)
                return (Enumerable.Empty<dynamic>(), 400, "Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            // ACCEPT BOTH camelCase & PascalCase
            string action = json["Action"]?.ToString() ?? json["action"]?.ToString();

            string serialNumber = json["SerialNumber"]?.ToString() ?? json["serialNumber"]?.ToString();

            if (string.IsNullOrWhiteSpace(action))
                return (Enumerable.Empty<dynamic>(), 400, "Action is required");

            // SP has no ELSE branch - an unknown Action returns no result set at all
            if (!AllowedActions.Contains(action, StringComparer.OrdinalIgnoreCase))
                return (Enumerable.Empty<dynamic>(), 400,
                    $"Invalid Action. Allowed values: {string.Join(", ", AllowedActions)}");

            if (action.Equals("INSERT", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrWhiteSpace(serialNumber))
                return (Enumerable.Empty<dynamic>(), 400, "SerialNumber is required for INSERT");

            var request = new
            {
                SerialNumber = serialNumber,
                Action = action,
                userid = userId
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
