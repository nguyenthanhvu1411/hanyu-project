namespace HanYu.Application.Features.Identity.Common;

public sealed record AuthResponse(
    Guid? UserId,
    Guid? PublicId,
    string? UserName,
    string? Email,
    string? DisplayName,
    IReadOnlyCollection<string>? Roles,
    string? AccessToken,
    DateTime? AccessTokenExpiresAtUtc,
    string? RefreshToken,
    bool RequiresTwoFactor = false,
    string? TwoFactorChallengeToken = null,
    DateTime? TwoFactorChallengeExpiresAtUtc = null)
{
    public static AuthResponse TwoFactorRequired(
        string challengeToken,
        DateTime expiresAtUtc)
    {
        return new AuthResponse(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            true,
            challengeToken,
            expiresAtUtc);
    }
}