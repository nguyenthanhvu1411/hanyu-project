using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;
using HanYu.Application.Interfaces.Authentication;
namespace HanYu.Application.Features.Identity.TwoFactor.Enable;

public sealed class EnableTwoFactorService
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ICurrentUserService _currentUserService;
    public EnableTwoFactorService(
        ITwoFactorService twoFactorService,
        ICurrentUserService currentUserService)
    {
        _twoFactorService = twoFactorService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TwoFactorRecoveryCodesResponse>> ExecuteAsync(
        EnableTwoFactorRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            return Result.Failure<TwoFactorRecoveryCodesResponse>(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Bạn chưa đăng nhập."));
        }

        return await _twoFactorService.EnableAsync(
            userId.Value,
            request.Code,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
