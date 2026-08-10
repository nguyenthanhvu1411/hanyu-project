using System.Security.Cryptography;
using System.Text;
using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HanYu.Infrastructure.Identity;

public sealed class AuthenticationTokenIssuer : IAuthenticationTokenIssuer
{
    private readonly UserManager<User> _userManager;
    private readonly HanYuDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthenticationTokenIssuer(
        UserManager<User> userManager,
        HanYuDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<Result<AuthResponse>> IssueAsync(
        User user,
        UserSession session,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        await EnsureSessionPersistedAsync(
            session,
            cancellationToken);

        var roles =
            await _userManager.GetRolesAsync(user);

        var access =
            _jwtTokenService.GenerateAccessToken(
                new JwtTokenUser(
                    user.Id,
                    user.Email ?? string.Empty,
                    roles.ToArray(),
                    session.SessionKey));

        var rawRefreshToken =
            _jwtTokenService.GenerateRefreshToken();

        var refreshToken =
            new RefreshToken(
                user.Id,
                session.Id,
                HashRefreshToken(rawRefreshToken),
                DateTimeOffset.UtcNow.AddDays(
                    _jwtOptions.RefreshTokenExpirationDays),
                ipAddress,
                userAgent,
                null);

        _dbContext
            .Set<RefreshToken>()
            .Add(refreshToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new AuthResponse(
                user.Id,
                user.PublicId,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                user.Profile?.DisplayName,
                roles.ToArray(),
                access.AccessToken,
                access.ExpiresAtUtc,
                rawRefreshToken));
    }

    private static string HashRefreshToken(
        string token)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes);
    }

    private async Task EnsureSessionPersistedAsync(
        UserSession session,
        CancellationToken cancellationToken)
    {
        if (session.Id > 0)
            return;

        var entry =
            _dbContext.Entry(session);

        if (entry.State == EntityState.Detached)
        {
            _dbContext
                .Set<UserSession>()
                .Add(session);
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        if (session.Id <= 0)
        {
            throw new InvalidOperationException(
                "Failed to persist user session.");
        }
    }
}
