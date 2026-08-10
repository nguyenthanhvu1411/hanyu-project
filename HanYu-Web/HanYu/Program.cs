using HanYu.API.Common.Middleware;
using HanYu.API.Extensions;
using HanYu.Application.Interfaces.Course;
using HanYu.Infrastructure;
using HanYu.Infrastructure.Course;
using HanYu.Infrastructure.Observability;
using HanYu.Infrastructure.Persistence;
using HanYu.Infrastructure.Security;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, config) =>
        config.ConfigureHanYu(ctx.Configuration, ctx.HostingEnvironment));

    builder.Services.AddControllers();
    builder.Services.AddHanYuSwagger();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

    // Curriculum reorder is intentionally isolated from the general Course service
    // because it performs two-phase writes to preserve unique sort-order constraints.
    builder.Services.AddScoped<
        ICourseCurriculumReorderService,
        CourseCurriculumReorderService>();

    var app = builder.Build();

    app.UseHanYuNetworkSecurity();

    if (app.Environment.IsDevelopment())
    {
        await app.Services.InitializeDatabaseAsync();
    }

    app.UseHanYuSwagger();

    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

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

        opts.GetLevel = (ctx, elapsed, ex) =>
            ctx.Request.Path.StartsWithSegments("/health")
                ? Serilog.Events.LogEventLevel.Debug
                : ex != null
                    ? Serilog.Events.LogEventLevel.Error
                    : ctx.Response.StatusCode >= 500
                        ? Serilog.Events.LogEventLevel.Error
                        : Serilog.Events.LogEventLevel.Information;
    });

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<SecurityHeadersMiddleware>();
    app.UseExceptionHandler();

    if (!app.Environment.IsEnvironment("IntegrationTest"))
    {
        app.UseRateLimiter();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseHanYuHealthChecks();
    app.UseHanYuMetrics();
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
