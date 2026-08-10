namespace HanYu.Infrastructure.Options;

/// <summary>
/// Production security configuration. Bind from environment variables or secrets manager.
/// Never commit real values to source control.
/// </summary>
public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    /// <summary>
    /// Allowed CORS origins. Example: ["https://hanyu.vn", "https://admin.hanyu.vn"]
    /// </summary>
    public string[] AllowedOrigins { get; init; } = [];

    /// <summary>
    /// Maximum request body size in bytes for standard JSON endpoints.
    /// Default: 2 MB.
    /// </summary>
    public long MaxRequestBodySizeBytes { get; init; } = 2 * 1024 * 1024;

    /// <summary>
    /// Maximum number of forwarded proxy hops to trust.
    /// Set to 1 when behind a single reverse proxy (Nginx/Cloudflare).
    /// </summary>
    public int ForwardedHeadersLimit { get; init; } = 1;

    /// <summary>
    /// Known proxy IP addresses to trust for X-Forwarded-For.
    /// Leave empty in development. Populate in production.
    /// </summary>
    public string[] KnownProxies { get; init; } = [];
}
