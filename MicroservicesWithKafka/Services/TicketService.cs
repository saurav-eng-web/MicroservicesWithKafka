using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;

        public TicketService(ITicketRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<dynamic>, int, string)> ProcessAsync(object payload)
        {
            if (payload == null)
                throw new ArgumentException("Payload is required");

            JObject json = payload is JsonElement el
                ? JObject.Parse(el.GetRawText())
                : JObject.FromObject(payload);

            string type = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None)
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}