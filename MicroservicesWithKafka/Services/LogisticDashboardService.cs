using MicroservicesWithKafka.Repository;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticDashboardService : ILogisticDashboardService
    {
        private static readonly string[] Buckets =
            { "PickupAwaited", "InTransit", "Delivered", "ChallanToBeCancel" };

        private readonly ILogisticDashboardRepository _repository;

        public LogisticDashboardService(ILogisticDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "summary", "details");

            if (type == "details")
            {
                var bucket = LogisticPayload.RequireStr(json, "bucket");
                if (Array.IndexOf(Buckets, bucket) < 0)
                    throw new ArgumentException("bucket must be one of " + string.Join(", ", Buckets) + ".");

                var rows = await _repository.GetDashboardDetailsAsync(bucket);
                return (rows, 200, "Record found Successfully");
            }

            var data = await _repository.GetDashboardSummaryAsync();
            return (data, 200, "Record found Successfully");
        }
    }
}
