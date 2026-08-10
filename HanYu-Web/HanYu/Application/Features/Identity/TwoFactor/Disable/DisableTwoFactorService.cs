using HanYu.Application.Common.Models;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.TwoFactor.Disable;

public sealed class DisableTwoFactorService
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ICurrentUserService _currentUserService;
    public DisableTwoFactorService(
        ITwoFactorService twoFactorService,
        ICurrentUserService currentUserService)
    {
        _twoFactorService = twoFactorService;
        _currentUserService = currentUserService;
    }

    public async Task<Result> ExecuteAsync(
        DisableTwoFactorRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            return Result.Failure(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Bạn chưa đăng nhập."));
        }

        return await _twoFactorService.DisableAsync(
            userId.Value,
            request.Password,
            request.Code,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
