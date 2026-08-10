using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Common;

namespace HanYu.Application.Features.Identity.TwoFactor.Login;

public sealed record TwoFactorLoginRequest(
    string ChallengeToken,
    string Code,
    string? IpAddress = null,
    string? UserAgent = null,
    string? DeviceName = null,
    string? DeviceType = null,
    string? Browser = null,
    string? OperatingSystem = null);
