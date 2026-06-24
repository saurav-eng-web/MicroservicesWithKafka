using MicroservicesWithKafka.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace MicroservicesWithKafka.Services
{
    // Finance operations -> SP_CRUD_GetFinanceClaimCounts.
    // Forwards the caller's dynamic payload (the proc dispatches on $.type, e.g. "financecounts"). userId from JWT.
    public class FinanceClaimCountService : IFinanceClaimCountService
    {
        private readonly IFinanceClaimCountRepository _repo;

        public FinanceClaimCountService(IFinanceClaimCountRepository repo)
        {
            _repo = repo;
        }

        public async Task<(List<List<dynamic>> DataSets, int StatusCode, string StatusMsg)> GetClaimCountsAsync(object payload, int userId)
        {
            JObject json = payload == null ? new JObject()
                : payload is JsonElement element ? JObject.Parse(element.GetRawText())
                : JObject.FromObject(payload);

            var request = new
            {
                payload = json.ToString(Formatting.None),   // forwarded as-is; $.type comes from the caller
                userId = userId                             // FROM JWT
            };

            return await _repo.ExecuteAsync(request);
        }
    }
}
