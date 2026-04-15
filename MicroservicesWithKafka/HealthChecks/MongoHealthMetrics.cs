using Prometheus;

namespace MicroservicesWithKafka.HealthChecks
{
    public static class MongoHealthMetrics
    {
        // 1 = healthy, 0 = down
        public static readonly Gauge MongoUp = Metrics.CreateGauge(
            "mongo_up",
            "MongoDB availability (1 = up, 0 = down)"
        );

        // Unix timestamp when Mongo first went down
        public static readonly Gauge MongoDownSince = Metrics.CreateGauge(
            "mongo_down_since",
            "Unix timestamp when MongoDB went down (0 if up)"
        );
    }
}
