using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.TwoFactor.Common;

namespace HanYu.Application.Features.Identity.TwoFactor.RegenerateKey;

public sealed record RegenerateAuthenticatorKeyRequest(
    string Password);
