using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Logout;

public sealed class LogoutService
{
    private readonly IIdentityService _identityService;

    public LogoutService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> ExecuteAsync(
        LogoutRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        return _identityService.LogoutAsync(
            request.RefreshToken,
            ipAddress,
            cancellationToken);
    }
}