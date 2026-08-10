namespace HanYu.Application.Common.Models;

public sealed record TwoFactorChallengeTokenResult(
    string Token,
    DateTime ExpiresAtUtc);
