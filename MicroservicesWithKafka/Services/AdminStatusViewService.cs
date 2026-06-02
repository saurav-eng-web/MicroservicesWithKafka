using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class AdminStatusViewService : IAdminStatusViewService
    {
        private readonly IAdminStatusViewRepository _repo;

        public AdminStatusViewService(IAdminStatusViewRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object payload)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            // Normalize payload into JObject handling System.Text.Json.JsonElement gracefully
            JObject json = payload is JsonElement el
                ? JObject.Parse(el.GetRawText())
                : JObject.FromObject(payload);

            // Stored procedure checks for camelCase or PascalCase via JSON_VALUE properties.
            // Using a fallback matching check here to guarantee Type validation
            string type = (json["Type"] ?? json["type"])?.ToString();
            string pk = (json["pk"] ?? json["PK"])?.ToString();
            string pk1 = (json["pk1"] ?? json["PK1"])?.ToString();

            if (string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(pk) && string.IsNullOrWhiteSpace(pk1))
            {
                throw new ArgumentException("Valid transaction parameter (Type, pk, or pk1) must be included in the payload.");
            }

            // Map standard text format straight to the Dapper parameter matching the target SP
            var request = new
            {
                payload = json.ToString(Formatting.None)
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}