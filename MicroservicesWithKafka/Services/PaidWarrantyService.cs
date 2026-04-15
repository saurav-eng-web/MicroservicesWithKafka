using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    public class PaidWarrantyService : IPaidWarrantyService
    {
        private readonly IPaidWarrantyRepository _repo;

        public PaidWarrantyService(IPaidWarrantyRepository repo)
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
                payload = json.ToString(Formatting.None), // FULL JSON
                sf_id = Convert.ToInt32(sfId)              // FROM JWT
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
