namespace HanYu.Application.Features.Identity.Sessions;

public sealed record UserSessionResponse(
    long Id,
    string? DeviceName,
    string? DeviceType,
    string? Browser,
    string? OperatingSystem,
    string? IpAddress,
    DateTime CreatedAtUtc,
    DateTime? LastAccessedAtUtc,
    bool IsCurrentSession);
