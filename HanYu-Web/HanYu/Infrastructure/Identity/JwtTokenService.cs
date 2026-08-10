using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Constants;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HanYu.Infrastructure.Identity;

public sealed class JwtTokenService : IJwtTokenService
{
    private const string TwoFactorPurpose =
        "two_factor";

    private readonly JwtOptions _options;

    public JwtTokenService(
        IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResult GenerateAccessToken(
        JwtTokenUser user)
    {
        var now = DateTime.UtcNow;

        var expiresAt = now.AddMinutes(
            _options.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Email,
                user.Email)
        };

        if (user.SessionKey.HasValue)
        {
            claims.Add(
                new Claim(
                    ClaimNames.SessionId,
                    user.SessionKey.Value.ToString()));
        }

        foreach (var role in user.Roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.SecretKey));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new JwtTokenResult(
            accessToken,
            expiresAt);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public TwoFactorChallengeTokenResult
        GenerateTwoFactorChallengeToken(
            Guid userId,
            string email)
    {
        var now = DateTime.UtcNow;

        var expiresAt =
            now.AddMinutes(5);

        var claims =
            new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    userId.ToString()),

                new(
                    JwtRegisteredClaimNames.Email,
                    email),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                new(
                    "purpose",
                    TwoFactorPurpose)
            };

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.SecretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAt,
                signingCredentials: credentials);

        return new TwoFactorChallengeTokenResult(
            new JwtSecurityTokenHandler()
                .WriteToken(token),
            expiresAt);
    }

    public Guid? ValidateTwoFactorChallengeToken(
        string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var tokenHandler =
                new JwtSecurityTokenHandler();

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _options.SecretKey));

            var principal =
                tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = _options.Issuer,

                        ValidateAudience = true,
                        ValidAudience = _options.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,

                        ValidateLifetime = true,

                        ClockSkew =
                            TimeSpan.FromSeconds(30)
                    },
                    out _);

            var purpose =
                principal.FindFirstValue(
                    "purpose");

            if (purpose !=
                TwoFactorPurpose)
            {
                return null;
            }

            var userIdValue =
                principal.FindFirstValue(
                    JwtRegisteredClaimNames.Sub)
                ?? principal.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                userIdValue,
                out var userId)
                ? userId
                : null;
        }
        catch
        {
            return null;
        }
    }
}