using Serilog;
using Serilog.Events;

namespace HanYu.Infrastructure.Observability;

/// <summary>
/// Configures structured logging with Serilog.
///
/// Design decisions:
/// - Structured JSON in Production (for log aggregation tools: Grafana Loki, ELK, Datadog)
/// - Human-readable console in Development
/// - Separate enrichers for CorrelationId and UserId (set by middleware)
/// - EF Core SQL queries logged only at Debug level (never in Production by default)
/// - Sensitive data (passwords, tokens, PII) MUST NOT be logged — enforced by level control
///
/// Log retention:
/// - File sink: rolling daily, 7-day retention, 50 MB max per file
/// - Console sink: all environments
/// - No file sink in Production (use centralized sink/forwarder instead)
/// </summary>
public static class SerilogConfiguration
{
    public static LoggerConfiguration ConfigureHanYu(
        this LoggerConfiguration config,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        config
            .ReadFrom.Configuration(configuration) // Override from appsettings
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "HanYu")
            .Enrich.WithProperty("Version",
                typeof(SerilogConfiguration).Assembly.GetName().Version?.ToString() ?? "unknown");

        if (environment.IsDevelopment())
        {
            // Human-readable output for local development
            config
                .WriteTo.Console(
                    outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} » {Message:lj} {Properties:j}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Debug)
                .WriteTo.File(
                    "logs/hanyu-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 50 * 1024 * 1024,
                    outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} » {Message:lj} {Properties:j}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Debug);
        }
        else
        {
            // JSON output for production log aggregation (Grafana Loki, ELK, Datadog, etc.)
            // Each log entry is a single-line JSON object, parseable by log ingestors.
            config
                .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter(renderMessage: true),
                    restrictedToMinimumLevel: LogEventLevel.Information);

            // In production, prefer a centralized log sink (Seq, Loki, Splunk).
            // File sink is provided as fallback but should be disabled when using centralized sink.
            // Uncomment if needed:
            // config.WriteTo.File(new JsonFormatter(renderMessage: true),
            //     "logs/hanyu-.json",
            //     rollingInterval: RollingInterval.Day,
            //     retainedFileCountLimit: 3,
            //     fileSizeLimitBytes: 100 * 1024 * 1024,
            //     restrictedToMinimumLevel: LogEventLevel.Information);
        }

        // ─── Minimum Level Overrides ──────────────────────────────────────────────
        // These prevent noisy framework logs from flooding the output.
        // EF Core SQL queries are Debug-only and suppressed in Production via appsettings.

        config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Information) // Log startup
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning) // No SQL in prod
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning);

        return config;
    }
}
