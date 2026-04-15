using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class SymptomCauseService : ISymptomCauseService
    {
        private readonly ISymptomCauseRepository _repo;

        public SymptomCauseService(ISymptomCauseRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement element
                ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            // accept camelCase & PascalCase
            string type =
                json["type"]?.ToString() ??
                json["Type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                sf_id = sfId,                              // FROM JWT
                payload = json.ToString(Formatting.None)  // FULL JSON
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
