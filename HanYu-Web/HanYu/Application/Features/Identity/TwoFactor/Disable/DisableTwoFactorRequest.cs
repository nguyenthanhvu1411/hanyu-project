using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Identity.TwoFactor.Disable;

public sealed record DisableTwoFactorRequest(
    string Password,
    string Code);
