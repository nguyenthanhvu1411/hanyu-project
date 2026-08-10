using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed record ConsentResponse(
    UserConsentType ConsentType,
    string Version,
    bool IsGranted,
    DateTimeOffset? GrantedAt,
    DateTimeOffset? RevokedAt);
