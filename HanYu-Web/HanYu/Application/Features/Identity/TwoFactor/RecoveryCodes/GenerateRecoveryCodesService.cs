using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.TwoFactor.RecoveryCodes;

public sealed class GenerateRecoveryCodesService
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ICurrentUserService _currentUserService;

    public GenerateRecoveryCodesService(
        ITwoFactorService twoFactorService,
        ICurrentUserService currentUserService)
    {
        _twoFactorService = twoFactorService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TwoFactorRecoveryCodesResponse>> ExecuteAsync(
        GenerateRecoveryCodesRequest request,
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

        return await _twoFactorService.GenerateRecoveryCodesAsync(
            userId.Value,
            request.Password,
            cancellationToken);
    }
}
