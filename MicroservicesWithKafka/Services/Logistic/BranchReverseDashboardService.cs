using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class BranchReverseDashboardService : IBranchReverseDashboardService
    {
        private static readonly string[] Buckets =
            { "PickupAwaited", "InTransit", "Delivered", "ChallanToBeCancel" };

        private readonly IBranchReverseDashboardRepository _repository;

        public BranchReverseDashboardService(IBranchReverseDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, int branchId)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "summary", "details");

            if (type == "details")
            {
                var bucket = LogisticPayload.RequireStr(json, "bucket");
                if (Array.IndexOf(Buckets, bucket) < 0)
                    throw new ArgumentException("bucket must be one of " + string.Join(", ", Buckets) + ".");

                var rows = await _repository.GetDetailsAsync(branchId, bucket);
                return (rows, 200, "Record found Successfully");
            }

            var data = await _repository.GetSummaryAsync(branchId);
            return (data, 200, "Record found Successfully");
        }
    }
}
