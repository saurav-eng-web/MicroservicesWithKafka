using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class EstimatedCostService : IEstimatedCostService
    {
        private readonly IEstimatedCostRepository _repo;

        public EstimatedCostService(IEstimatedCostRepository repo)
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

            string type = json["type"]?.ToString();

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("type is required");

            var request = new
            {
                payload = json.ToString(Formatting.None),
                sf_id = Convert.ToInt32(sfId)
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}

