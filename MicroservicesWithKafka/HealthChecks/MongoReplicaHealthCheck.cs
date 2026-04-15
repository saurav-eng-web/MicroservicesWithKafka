using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using Prometheus;

namespace MicroservicesWithKafka.HealthChecks
{
    public class MongoReplicaHealthCheck : IHealthCheck
    {
        private readonly MongoClient _client;
        private static DateTime? _lastDownTime;

        public MongoReplicaHealthCheck(IConfiguration config)
        {
            _client = new MongoClient(config["FundsDatabase:ConnectionString"]);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var adminDb = _client.GetDatabase("admin");

                var result = await adminDb.RunCommandAsync<BsonDocument>(
                    new BsonDocument("replSetGetStatus", 1),
                    cancellationToken: cancellationToken
                );

                int myState = result["myState"].AsInt32;
                string role = myState == 1 ? "PRIMARY" : "SECONDARY";

                // Mongo OK
                _lastDownTime = null;
                MongoHealthMetrics.MongoUp.Set(1);
                MongoHealthMetrics.MongoDownSince.Set(0);

                return HealthCheckResult.Healthy(
                    $"Mongo ReplicaSet OK ({role})"
                );
            }
            catch (Exception ex)
            {
                // Mongo DOWN
                _lastDownTime ??= DateTime.UtcNow;

                MongoHealthMetrics.MongoUp.Set(0);
                MongoHealthMetrics.MongoDownSince.Set(
                    new DateTimeOffset(_lastDownTime.Value).ToUnixTimeSeconds()
                );

                return HealthCheckResult.Unhealthy(
                    $"Mongo DOWN | Down since: {_lastDownTime:yyyy-MM-dd HH:mm:ss} UTC",
                    ex
                );
            }
        }
    }
}
