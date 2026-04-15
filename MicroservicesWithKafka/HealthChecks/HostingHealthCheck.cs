using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MicroservicesWithKafka.HealthChecks
{
    public class HostingHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("API service is running")
            );
        }
    }
}
