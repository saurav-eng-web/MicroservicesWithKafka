using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace MicroservicesWithKafka.HealthChecks
{
    public class KafkaReplicaHealthCheck : IHealthCheck
    {
        private static DateTime? _lastDownTime;

        private readonly IConfiguration _config;

        public KafkaReplicaHealthCheck(IConfiguration config)
        {
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var adminConfig = new AdminClientConfig
                {
                    BootstrapServers = _config["Kafka:BootstrapServers"]
                };

                using var admin = new AdminClientBuilder(adminConfig).Build();
                var meta = admin.GetMetadata(TimeSpan.FromSeconds(3));

                // Kafka OK
                _lastDownTime = null;
                KafkaHealthMetrics.KafkaUp.Set(1);
                KafkaHealthMetrics.KafkaDownSince.Set(0);

                return HealthCheckResult.Healthy(
                    $"Kafka OK | Brokers: {meta.Brokers.Count}"
                );
            }
            catch (Exception ex)
            {
                // Kafka DOWN
                _lastDownTime ??= DateTime.UtcNow;

                KafkaHealthMetrics.KafkaUp.Set(0);
                KafkaHealthMetrics.KafkaDownSince.Set(
                    new DateTimeOffset(_lastDownTime.Value).ToUnixTimeSeconds()
                );

                return HealthCheckResult.Unhealthy(
                    $"Kafka DOWN | Down since: {_lastDownTime:yyyy-MM-dd HH:mm:ss} UTC",
                    ex
                );
            }
        }
    }
}
