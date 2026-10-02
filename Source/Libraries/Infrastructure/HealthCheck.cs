using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure;

public class DatabaseHealthCheck(ApplicationContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.Database.CanConnectAsync(cancellationToken);
        return result switch
        {
            true => HealthCheckResult.Healthy(),
            false => HealthCheckResult.Unhealthy()
        };
    }
}
