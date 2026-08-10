namespace HanYu.Application.Features.Identity.Admin.Sessions;

public sealed record AdminSessionDto
{
    public long Id { get; init; }
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string UserDisplayName { get; init; } = string.Empty;
    public string? DeviceInfo { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? LastUsedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public DateTimeOffset? RevokedAt { get; init; }
    public string? RevokedReason { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset? DeletedAt { get; init; }
    public string? ConcurrencyToken { get; init; }
}
