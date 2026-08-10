using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.TwoFactor.Setup;

public sealed class SetupTwoFactorService
{
    private readonly ITwoFactorService _twoFactorService;
    private readonly ICurrentUserService _currentUserService;

    public SetupTwoFactorService(
        ITwoFactorService twoFactorService,
        ICurrentUserService currentUserService)
    {
        _twoFactorService = twoFactorService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<TwoFactorSetupResponse>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue || userId.Value == Guid.Empty)
        {
            return Result.Failure<TwoFactorSetupResponse>(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Bạn chưa đăng nhập."));
        }

        return await _twoFactorService.SetupAsync(
            userId.Value,
            cancellationToken);
    }
}
