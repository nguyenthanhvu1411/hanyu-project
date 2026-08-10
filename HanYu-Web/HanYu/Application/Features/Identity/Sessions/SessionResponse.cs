namespace HanYu.Application.Features.Identity.Sessions;

public sealed record SessionResponse(
    Guid SessionKey,
    string? DeviceName,
    string? DeviceType,
    string? Browser,
    string? OperatingSystem,
    string? IpAddress,
    DateTimeOffset LastActivityAt,
    DateTimeOffset? RevokedAt,
    string Status,
    bool IsCurrent);
