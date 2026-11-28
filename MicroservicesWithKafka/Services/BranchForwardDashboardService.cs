using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class BranchForwardDashboardService : IBranchForwardDashboardService
    {
        private static readonly string[] Buckets =
            { "PickupAwaited", "InTransit", "Delivered", "ReceiptPending" };

        private readonly IBranchForwardDashboardRepository _repository;

        public BranchForwardDashboardService(IBranchForwardDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, DashboardScope scope)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "summary", "details", "challan");

            if (type == "details")
            {
                var bucket = LogisticPayload.RequireStr(json, "bucket");
                if (Array.IndexOf(Buckets, bucket) < 0)
                    throw new ArgumentException("bucket must be one of " + string.Join(", ", Buckets) + ".");

                return (await _repository.GetDetailsAsync(scope, bucket), 200, "Record found Successfully");
            }

            if (type == "challan")
            {
                var challanNo = LogisticPayload.RequireStr(json, "challanNo");
                if (challanNo.Length > 200)
                    throw new ArgumentException("challanNo is too long.");

                return (await _repository.GetChallanAsync(scope, challanNo), 200, "Record found Successfully");
            }

            return (await _repository.GetSummaryAsync(scope), 200, "Record found Successfully");
        }
    }
}
