using HanYu.API.Common.Middleware;
using HanYu.API.Extensions;
using HanYu.Infrastructure;
using HanYu.Infrastructure.Observability;
using HanYu.Infrastructure.Persistence;
using HanYu.Infrastructure.Security;
using Microsoft.Extensions.Hosting;
using Serilog;

// ─── Bootstrap Logger ────────────────────────────────────────────────────────
// Catches fatal startup errors (e.g., DB connection failure at boot) before DI is built.
// Replaced by the fully configured Serilog logger after builder.Build().
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ─────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, services, config) =>
        config.ConfigureHanYu(ctx.Configuration, ctx.HostingEnvironment));

    // ==============================
    // Services
    // ==============================

    builder.Services.AddControllers();

    builder.Services.AddHanYuSwagger();

    builder.Services.AddProblemDetails();

    builder.Services.AddExceptionHandler<
        GlobalExceptionHandler>();

    builder.Services.AddInfrastructure(
        builder.Configuration,
        builder.Environment); // overload also registers OpenTelemetry

    // ==============================
    // Build
    // ==============================

    var app = builder.Build();

    // ==============================
    // HTTP Pipeline
    // Order matters — see inline comments.
    // ==============================

    // 1. Resolve real client IP/scheme from proxy headers (must be FIRST)
    //    UseForwardedHeaders + HTTPS + HSTS + CORS
    app.UseHanYuNetworkSecurity();

    // 2. Development-only: seed reference data + Swagger UI
    if (app.Environment.IsDevelopment())
    {
        await app.Services.InitializeDatabaseAsync();
    }

    app.UseHanYuSwagger(); // internally guarded: only active in Development

    // 3. Serilog request logging (structured HTTP request log per request)
    //    Placed after security middleware so real IP is already resolved.
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

        // Enrich each request log with additional context
        opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId is not null)
                {
                    diagnosticContext.Set("UserId", userId);
                }
            }

            var correlationId = httpContext.Response.Headers["X-Correlation-Id"].ToString();
            if (!string.IsNullOrEmpty(correlationId))
            {
                diagnosticContext.Set("CorrelationId", correlationId);
            }
        };

        // Exclude noisy health-check paths from request logs
        opts.GetLevel = (ctx, elapsed, ex) =>
            ctx.Request.Path.StartsWithSegments("/health")
                ? Serilog.Events.LogEventLevel.Debug
                : ex != null
                    ? Serilog.Events.LogEventLevel.Error
                    : ctx.Response.StatusCode >= 500
                        ? Serilog.Events.LogEventLevel.Error
                        : Serilog.Events.LogEventLevel.Information;
    });

    // 4. Correlation ID: inject tracing header early so it appears in all log entries
    app.UseMiddleware<CorrelationIdMiddleware>();

    // 5. Security headers on every response
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // 6. Global error handling (after security headers so headers still appear on 500s)
    app.UseExceptionHandler();

    // 7. Rate limiting. Integration tests exercise business/API behavior separately
    //    and must not share production per-IP limiter state across test cases.
    if (!app.Environment.IsEnvironment("IntegrationTest"))
    {
        app.UseRateLimiter();
    }

    // 8. Auth
    app.UseAuthentication();
    app.UseAuthorization();

    // 9. Health checks — no auth required, minimal response
    app.UseHanYuHealthChecks();

    // 10. Prometheus metrics scrape endpoint — restrict access via reverse proxy
    app.UseHanYuMetrics();

    // 11. Controllers
    app.MapControllers();

    await app.RunAsync();

    return 0;
}
catch (Exception ex) when (
    ex is not OperationCanceledException and
    not HostAbortedException)
{
    Log.Fatal(ex, "Application startup failed.");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

public partial class Program { }
