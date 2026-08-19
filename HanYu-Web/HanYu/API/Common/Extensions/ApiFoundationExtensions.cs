namespace HanYu.API.Common.Extensions;

public static class ApiFoundationExtensions
{
    // Must match RateLimitingExtensions.AdminWrite registered by AddHanYuRateLimiting().
    public const string AdminWriteRateLimitPolicy = "rl_admin_write";
}
