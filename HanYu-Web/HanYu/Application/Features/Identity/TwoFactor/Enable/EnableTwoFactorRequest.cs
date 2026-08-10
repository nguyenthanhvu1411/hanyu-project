using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;

namespace HanYu.Application.Features.Identity.TwoFactor.Enable;

public sealed record EnableTwoFactorRequest(
    string Code);
