using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class PrintJobsheetService : IPrintJobsheetService
    {
        private readonly IPrintJobsheetRepository _repo;

        public PrintJobsheetService(IPrintJobsheetRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(object payload, string sfId)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement el ? JObject.Parse(el.GetRawText()): JObject.FromObject(payload);

            string type = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None),
                sf_id = sfId
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
