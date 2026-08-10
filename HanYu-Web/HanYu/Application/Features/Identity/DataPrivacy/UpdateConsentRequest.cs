using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Identity.DataPrivacy;

public sealed record UpdateConsentRequest(
    UserConsentType ConsentType,
    string Version,
    bool IsGranted);
