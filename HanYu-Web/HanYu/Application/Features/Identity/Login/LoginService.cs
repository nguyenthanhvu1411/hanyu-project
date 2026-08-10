using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Login;

public sealed class LoginService
{
    private readonly IIdentityService _identityService;

    public LoginService(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result<AuthResponse>> ExecuteAsync(
        LoginRequest request,
        LoginClientInfo client,
        CancellationToken cancellationToken = default)
    {
        var identityRequest =
            new LoginIdentityRequest(
                request.Email,
                request.Password,
                client.IpAddress,
                client.UserAgent,
                client.DeviceName,
                client.DeviceType,
                client.Browser,
                client.OperatingSystem);

        return _identityService.LoginAsync(
            identityRequest,
            cancellationToken);
    }
}

public sealed record LoginClientInfo(
    string? IpAddress,
    string? UserAgent,
    string? DeviceName = null,
    string? DeviceType = null,
    string? Browser = null,
    string? OperatingSystem = null);