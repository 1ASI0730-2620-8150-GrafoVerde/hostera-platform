using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HosteraBackend.API.Shared.Infrastructure.Health;

/// <summary>
///     Reports the API as healthy when it can connect to its database.
/// </summary>
/// <param name="context">The <see cref="AppDbContext" /> whose database is checked.</param>
public class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        return await context.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("The database is not reachable.");
    }
}
