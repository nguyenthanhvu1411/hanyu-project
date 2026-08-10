namespace HanYu.Application.Features.Identity.RevokeSession;

public sealed record RevokeSessionRequest(
    Guid SessionKey);
