using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.RefreshToken;

public sealed class RefreshTokenService
{
    private readonly IIdentityService _identityService;

    public RefreshTokenService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<AuthResponse>> ExecuteAsync(
        RefreshTokenRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var identityRequest =
            new RefreshIdentityTokenRequest(
                request.RefreshToken,
                ipAddress,
                userAgent);

        return _identityService.RefreshTokenAsync(
            identityRequest,
            cancellationToken);
    }
}