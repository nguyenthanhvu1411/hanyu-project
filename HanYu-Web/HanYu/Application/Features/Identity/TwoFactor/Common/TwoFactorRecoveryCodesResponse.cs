namespace HanYu.Application.Features.Identity.TwoFactor.Common;

public sealed record TwoFactorRecoveryCodesResponse(
    IReadOnlyCollection<string> RecoveryCodes);
