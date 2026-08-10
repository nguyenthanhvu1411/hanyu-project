using HanYu.Application.Common.Models;

namespace HanYu.Application.Interfaces.Authentication;


public interface IJwtTokenService
{
    JwtTokenResult GenerateAccessToken(
        JwtTokenUser user);

    string GenerateRefreshToken();

    TwoFactorChallengeTokenResult
        GenerateTwoFactorChallengeToken(
            Guid userId,
            string email);

    Guid? ValidateTwoFactorChallengeToken(
        string token);
}

public sealed record JwtTokenUser(
    Guid Id,
    string Email,
    IReadOnlyCollection<string> Roles,
    Guid? SessionKey = null);

public sealed record JwtTokenResult(
    string AccessToken,
    System.DateTime ExpiresAtUtc);