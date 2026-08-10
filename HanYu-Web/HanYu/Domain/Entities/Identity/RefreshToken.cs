using HanYu.Domain.Entities;

namespace HanYu.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }

    public long? UserSessionId { get; private set; }

    public string TokenHash { get; private set; }
        = string.Empty;

    public Guid FamilyId { get; private set; }
        = Guid.NewGuid();

    public DateTimeOffset IssuedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? UsedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public long? ReplacedByTokenId { get; private set; }

    public string? CreatedByIp { get; private set; }

    public string? RevokedByIp { get; private set; }

    public string? UserAgent { get; private set; }

    public string? RevokeReason { get; private set; }

    public User User { get; private set; } = null!;

    public UserSession? UserSession { get; private set; }

    public bool IsExpired =>
        ExpiresAt <= DateTimeOffset.UtcNow;

    public bool IsRevoked =>
        RevokedAt.HasValue;

    public bool IsUsed =>
        UsedAt.HasValue;

    public bool IsActive =>
        !IsExpired &&
        !IsRevoked &&
        !IsUsed;

    protected RefreshToken()
    {
    }

    public RefreshToken(
        Guid userId,
        long? userSessionId,
        string tokenHash,
        DateTimeOffset expiresAt,
        string? createdByIp = null,
        string? userAgent = null,
        Guid? familyId = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (userSessionId.HasValue &&
            userSessionId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(userSessionId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException(
                "TokenHash không được để trống.",
                nameof(tokenHash));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentOutOfRangeException(
                nameof(expiresAt),
                "Refresh token phải hết hạn trong tương lai.");

        if (familyId.HasValue &&
            familyId.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "FamilyId không hợp lệ.",
                nameof(familyId));
        }

        UserId = userId;
        UserSessionId = userSessionId;
        TokenHash = tokenHash.Trim();
        ExpiresAt = expiresAt;

        FamilyId =
            familyId ?? Guid.NewGuid();

        CreatedByIp =
            Normalize(createdByIp);

        UserAgent =
            Normalize(userAgent);
    }

    public void MarkUsed(
        long? replacedByTokenId = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException(
                "Refresh token đã bị revoke.");

        if (IsExpired)
            throw new InvalidOperationException(
                "Refresh token đã hết hạn.");

        if (IsUsed)
            throw new InvalidOperationException(
                "Refresh token đã được sử dụng.");

        if (replacedByTokenId.HasValue &&
            replacedByTokenId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(replacedByTokenId));
        }

        UsedAt = DateTimeOffset.UtcNow;
        ReplacedByTokenId =
            replacedByTokenId;
    }

    public void Revoke(
        string? revokedByIp,
        string? reason)
    {
        if (IsRevoked)
            return;

        RevokedAt =
            DateTimeOffset.UtcNow;

        RevokedByIp =
            Normalize(revokedByIp);

        RevokeReason =
            Normalize(reason);
    }

    public void ReplaceBy(
        long replacementTokenId)
    {
        if (replacementTokenId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(replacementTokenId));

        ReplacedByTokenId =
            replacementTokenId;

        UsedAt ??=
            DateTimeOffset.UtcNow;
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
