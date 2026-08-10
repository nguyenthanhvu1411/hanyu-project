using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Features.Identity.Login;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.TwoFactor.Login;

public sealed class TwoFactorLoginService
{
    private readonly ITwoFactorService _twoFactorService;

    public TwoFactorLoginService(
        ITwoFactorService twoFactorService)
    {
        _twoFactorService = twoFactorService;
    }

    public async Task<Result<AuthResponse>> ExecuteAsync(
        TwoFactorLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var clientInfo = new LoginClientInfo(
            request.IpAddress,
            request.UserAgent,
            request.DeviceName,
            request.DeviceType,
            request.Browser,
            request.OperatingSystem);

        return await _twoFactorService.CompleteLoginAsync(
            request.ChallengeToken,
            request.Code,
            clientInfo,
            cancellationToken);
    }
}
