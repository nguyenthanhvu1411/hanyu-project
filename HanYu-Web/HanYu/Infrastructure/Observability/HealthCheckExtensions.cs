using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HanYu.Infrastructure.Observability;

/// <summary>
/// Health check configuration.
///
/// Endpoints:
///   GET /health/live  — Liveness: process is up (no DB check; used by container restart policy)
///   GET /health/ready — Readiness: DB and critical deps are available (used by load balancer)
///
/// Response does NOT expose connection strings or internal details.
/// </summary>
public static class HealthCheckExtensions
{
    public static IServiceCollection AddHanYuHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? string.Empty;

        services
            .AddHealthChecks()
            // EF Core DbContext check (readiness)
            .AddDbContextCheck<HanYuDbContext>(
                name: "db_ef",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            // Direct Npgsql check (readiness)
            .AddNpgSql(
                connectionString,
                name: "db_postgres",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    public static WebApplication UseHanYuHealthChecks(
        this WebApplication app)
    {
        // Liveness: just checks process is running (no external deps)
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            // Only run checks tagged "live" — none currently, so always healthy
            Predicate     = check => check.Tags.Contains("live"),
            ResponseWriter = WriteMinimalResponse
        });

        // Readiness: checks DB connectivity
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate      = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteMinimalResponse
        });

        return app;
    }

    /// <summary>
    /// Returns minimal JSON response — no connection strings, no server details.
    /// Example: {"status":"Healthy"}
    /// </summary>
    private static Task WriteMinimalResponse(
        Microsoft.AspNetCore.Http.HttpContext context,
        HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var status = report.Status.ToString();
        var body   = $"{{\"status\":\"{status}\"}}";

        return context.Response.WriteAsync(body);
    }
}
