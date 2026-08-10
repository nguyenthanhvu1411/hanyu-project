namespace HanYu.Application.Features.Identity.TwoFactor.Common;

public sealed record TwoFactorSetupResponse(
    string SharedKey,
    string AuthenticatorUri);
