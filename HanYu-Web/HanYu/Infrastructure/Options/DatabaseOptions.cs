namespace HanYu.Infrastructure.Options;

/// <summary>
/// Database connection and pool configuration.
/// </summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// Maximum connections per pool instance.
    /// Keep low for shared hosting (e.g., Neon). Default: 20.
    /// </summary>
    public int MaxPoolSize { get; init; } = 20;

    /// <summary>
    /// Minimum idle connections. Default 0 to avoid wasting connections on Neon.
    /// </summary>
    public int MinPoolSize { get; init; } = 0;

    /// <summary>
    /// Connection acquisition timeout in seconds. Default: 15.
    /// </summary>
    public int ConnectionTimeoutSeconds { get; init; } = 15;

    /// <summary>
    /// Per-command execution timeout in seconds. Default: 30.
    /// Heavy admin reports should override per-query, not raise this globally.
    /// </summary>
    public int CommandTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Maximum retry attempts on transient failures. Default: 3.
    /// </summary>
    public int MaxRetryCount { get; init; } = 3;

    /// <summary>
    /// Maximum delay between retries in seconds. Default: 5.
    /// </summary>
    public int MaxRetryDelaySeconds { get; init; } = 5;

    /// <summary>
    /// Whether to run EF Core migrations on startup.
    /// Enable only in Development. Production must run migrations via CI/CD.
    /// </summary>
    public bool AutoMigrateOnStartup { get; init; } = false;
}
