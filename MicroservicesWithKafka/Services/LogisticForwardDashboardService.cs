using MicroservicesWithKafka.Repository;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class LogisticForwardDashboardService : ILogisticForwardDashboardService
    {
        private static readonly string[] Buckets =
            { "PickupAwaited", "InTransit", "Delivered", "ReceiptPending" };

        private readonly ILogisticForwardDashboardRepository _repository;

        public LogisticForwardDashboardService(ILogisticForwardDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "summary", "details", "challan");

            if (type == "details")
            {
                var bucket = LogisticPayload.RequireStr(json, "bucket");
                if (Array.IndexOf(Buckets, bucket) < 0)
                    throw new ArgumentException("bucket must be one of " + string.Join(", ", Buckets) + ".");

                return (await _repository.GetDetailsAsync(bucket), 200, "Record found Successfully");
            }

            if (type == "challan")
            {
                var challanNo = LogisticPayload.RequireStr(json, "challanNo");
                if (challanNo.Length > 200)
                    throw new ArgumentException("challanNo is too long.");

                return (await _repository.GetChallanAsync(challanNo), 200, "Record found Successfully");
            }

            return (await _repository.GetSummaryAsync(), 200, "Record found Successfully");
        }
    }
}
