using Prometheus;

namespace MicroservicesWithKafka.HealthChecks
{
    public static class KafkaHealthMetrics
    {
        // 1 = healthy, 0 = down
        public static readonly Gauge KafkaUp = Metrics.CreateGauge(
            "kafka_up",
            "Kafka availability (1 = up, 0 = down)"
        );

        // Unix timestamp when Kafka first went down
        public static readonly Gauge KafkaDownSince = Metrics.CreateGauge(
            "kafka_down_since",
            "Unix timestamp when Kafka went down (0 if up)"
        );
    }
}
