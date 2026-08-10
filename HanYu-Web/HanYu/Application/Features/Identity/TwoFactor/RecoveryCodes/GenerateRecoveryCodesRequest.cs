using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;

namespace HanYu.Application.Features.Identity.TwoFactor.RecoveryCodes;

public sealed record GenerateRecoveryCodesRequest(
    string Password);
