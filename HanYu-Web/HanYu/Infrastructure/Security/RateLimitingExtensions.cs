using System.Threading.RateLimiting;
using HanYu.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Security;

/// <summary>
/// Registers tiered rate limiting policies based on OWASP recommendations.
/// Policies use the real client IP resolved from X-Forwarded-For (after ForwardedHeaders middleware).
/// </summary>
public static class RateLimitingExtensions
{
    // ─── Policy Names ───────────────────────────────────────────────────────────

    public const string Auth        = "rl_auth";
    public const string Register    = "rl_register";
    public const string ForgotPassword = "rl_forgot_password";
    public const string AiApi       = "rl_ai";
    public const string ReviewApi   = "rl_review";
    public const string EventsApi   = "rl_events";
    public const string SearchApi   = "rl_search";
    public const string AdminWrite  = "rl_admin_write";
    public const string Default     = "rl_default";

    // ─── Registration ────────────────────────────────────────────────────────────

    public static IServiceCollection AddHanYuRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(opts =>
        {
            // Default: reject with 429 Too Many Requests
            opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Per-IP policies for auth endpoints
            opts.AddPolicy(Auth, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetClientIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 5,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(Register, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetClientIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 3,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(ForgotPassword, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetClientIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 3,
                        Window            = TimeSpan.FromHours(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            // Per-user policies (fallback to IP if not authenticated)
            opts.AddPolicy(AiApi, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 15,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(ReviewApi, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 30,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(EventsApi, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 150,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 2
                    }));

            opts.AddPolicy(SearchApi, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 60,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(AdminWrite, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 60,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));

            opts.AddPolicy(Default, ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    GetUserOrIp(ctx),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit       = 100,
                        Window            = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit        = 0
                    }));
        });

        return services;
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────────

    private static string GetClientIp(HttpContext ctx) =>
        ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static string GetUserOrIp(HttpContext ctx) =>
        ctx.User.Identity?.IsAuthenticated == true
            ? ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
              ?? GetClientIp(ctx)
            : GetClientIp(ctx);
}
