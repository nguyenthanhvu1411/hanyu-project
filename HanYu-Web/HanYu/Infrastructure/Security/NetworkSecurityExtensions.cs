using System.Net;
using HanYu.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Security;

/// <summary>
/// Configures network-level security: ForwardedHeaders, CORS, HTTPS/HSTS.
/// 
/// Middleware execution order must be:
///   UseForwardedHeaders → UseExceptionHandler → UseHsts → UseHttpsRedirection → UseCors
/// </summary>
public static class NetworkSecurityExtensions
{
    public const string ProductionCorsPolicy    = "HanYu.Production";
    public const string DevelopmentCorsPolicy   = "HanYu.Development";

    // ─── Service Registration ────────────────────────────────────────────────────

    public static IServiceCollection AddHanYuNetworkSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var securityOptions = configuration
            .GetSection(SecurityOptions.SectionName)
            .Get<SecurityOptions>() ?? new SecurityOptions();

        // ForwardedHeaders: trust X-Forwarded-For and X-Forwarded-Proto
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto;

            // Limit depth of proxy hops to prevent header spoofing.
            options.ForwardLimit = securityOptions.ForwardedHeadersLimit;

            // Only trust known proxies. Clear defaults and re-add.
            // In production, populate SecurityOptions:KnownProxies from environment.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (var ip in securityOptions.KnownProxies)
            {
                if (IPAddress.TryParse(ip, out var address))
                {
                    options.KnownProxies.Add(address);
                }
            }

            // When KnownProxies is empty (e.g., development or single-instance Render),
            // all proxies are trusted. Acceptable for single-hop setups.
            // For multi-hop production (Cloudflare → Nginx → Kestrel), populate KnownProxies.
        });

        // CORS
        services.AddCors(options =>
        {
            // Production: lock down to specific origins
            options.AddPolicy(ProductionCorsPolicy, policy =>
            {
                var origins = securityOptions.AllowedOrigins;

                if (origins.Length > 0)
                {
                    policy
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    // No origins configured → deny all (safest default)
                    policy.SetIsOriginAllowed(_ => false);
                }
            });

            // Development: open CORS for local tooling
            options.AddPolicy(DevelopmentCorsPolicy, policy =>
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    // ─── Middleware Pipeline ─────────────────────────────────────────────────────

    /// <summary>
    /// Applies ForwardedHeaders, HSTS, HttpsRedirection and CORS in the correct order.
    /// MUST be called before UseAuthentication and UseAuthorization.
    /// </summary>
    public static WebApplication UseHanYuNetworkSecurity(
        this WebApplication app)
    {
        // 1. Resolve real client IP / scheme FIRST — everything else depends on it.
        app.UseForwardedHeaders();

        // 2. HSTS — only in non-development. Never in development (no valid TLS cert).
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // 3. Redirect HTTP → HTTPS
        app.UseHttpsRedirection();

        // 4. CORS — must come before Authentication
        var corsPolicy = app.Environment.IsDevelopment()
            ? DevelopmentCorsPolicy
            : ProductionCorsPolicy;

        app.UseCors(corsPolicy);

        return app;
    }
}
