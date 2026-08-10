using HanYu.Domain.Entities;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Identity;

public class UserConsent : BaseEntity
{
    public Guid UserId { get; private set; }

    public UserConsentType ConsentType { get; private set; }

    public string Version { get; private set; }
        = string.Empty;

    public bool IsGranted { get; private set; }

    public DateTimeOffset? GrantedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public User User { get; private set; } = null!;

    protected UserConsent()
    {
    }

    public UserConsent(
        Guid userId,
        UserConsentType consentType,
        string version,
        bool grantImmediately = true)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException(
                "Consent version không được để trống.",
                nameof(version));

        version = version.Trim();

        if (version.Length > 50)
            throw new ArgumentException(
                "Consent version không được vượt quá 50 ký tự.",
                nameof(version));

        UserId = userId;
        ConsentType = consentType;
        Version = version;

        if (grantImmediately)
            Grant();
    }

    public void Grant()
    {
        if (IsGranted)
            return;

        IsGranted = true;

        GrantedAt =
            DateTimeOffset.UtcNow;

        RevokedAt = null;
    }

    public void Revoke()
    {
        if (!IsGranted)
            return;

        IsGranted = false;

        RevokedAt =
            DateTimeOffset.UtcNow;
    }

    public void AcceptNewVersion(
        string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException(
                "Consent version không được để trống.",
                nameof(version));

        version = version.Trim();

        if (version.Length > 50)
            throw new ArgumentException(
                "Consent version không được vượt quá 50 ký tự.",
                nameof(version));

        Version = version;

        IsGranted = true;
        GrantedAt = DateTimeOffset.UtcNow;
        RevokedAt = null;
    }
}
