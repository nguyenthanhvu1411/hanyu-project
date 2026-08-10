using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HanYu.Infrastructure.Observability;

/// <summary>
/// Configures OpenTelemetry for distributed tracing and metrics.
///
/// Exporters by environment:
///   Development → Console exporter (stdout) for local inspection
///   Production  → OTLP exporter (Grafana Tempo, Jaeger, or any OTel-compatible backend)
///                 Configure via environment variable: OTEL_EXPORTER_OTLP_ENDPOINT
///
/// Metrics are exposed at /metrics (Prometheus scrape endpoint).
/// The endpoint should be protected from public access via reverse proxy allow-list.
///
/// Instrumentation:
///   - ASP.NET Core HTTP request traces
///   - HttpClient outgoing request traces (AI provider calls, SMS, Email)
///   - EF Core query traces (command text is sanitized; no PII in spans)
/// </summary>
public static class OpenTelemetryExtensions
{
    private const string ServiceName = "HanYu.Api";

    public static IServiceCollection AddHanYuOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var serviceVersion =
            typeof(OpenTelemetryExtensions).Assembly.GetName().Version?.ToString()
            ?? "0.0.0";

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource
                    .AddService(
                        serviceName: ServiceName,
                        serviceVersion: serviceVersion)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = environment.EnvironmentName,
                        ["host.name"] = Environment.MachineName
                    }))
            .WithTracing(tracing =>
            {
                tracing
                    // Instrument incoming HTTP requests
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        // Record exceptions as span events
                        opts.RecordException = true;

                        // Exclude health check endpoints from traces — too noisy
                        opts.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health") &&
                            !ctx.Request.Path.StartsWithSegments("/metrics");
                    })

                    // Instrument outgoing HTTP calls (AI providers, Twilio, S3, SMTP)
                    .AddHttpClientInstrumentation(opts =>
                    {
                        // Don't record response body — could contain PII or API keys in error messages
                        opts.RecordException = true;
                    })

                    // Instrument EF Core queries (command text without parameters — no PII)
                    .AddEntityFrameworkCoreInstrumentation(opts =>
                    {
                        // Set this to false in prod to avoid logging SQL (could contain data)
                        opts.SetDbStatementForText = environment.IsDevelopment();
                        opts.SetDbStatementForStoredProcedure = false;
                    });

                if (environment.IsDevelopment())
                {
                    // Console exporter for local inspection — not for production
                    tracing.AddConsoleExporter();
                }
                else
                {
                    // OTLP: configure via environment variables:
                    //   OTEL_EXPORTER_OTLP_ENDPOINT=https://tempo.your-domain.com
                    //   OTEL_EXPORTER_OTLP_HEADERS=Authorization=Bearer <token>
                    //
                    // Or use Grafana Cloud / Honeycomb / Datadog OTLP endpoint.
                    tracing.AddOtlpExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter(); // Exposed at /metrics
            });

        return services;
    }

    /// <summary>
    /// Maps the Prometheus /metrics scrape endpoint.
    /// IMPORTANT: Restrict access at reverse proxy level — do not expose publicly.
    /// Example Nginx: allow 10.0.0.0/8; deny all;
    /// </summary>
    public static WebApplication UseHanYuMetrics(
        this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint("/metrics");
        return app;
    }
}
