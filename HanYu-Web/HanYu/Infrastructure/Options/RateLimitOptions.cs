namespace HanYu.Infrastructure.Options;

public sealed class RateLimitOptions
{
    public const string SectionName =
        "RateLimit";

    public int DefaultPermitLimit { get; init; } =
        100;

    public int LoginPermitLimit { get; init; } =
        5;

    public int WindowSeconds { get; init; } =
        60;
}
