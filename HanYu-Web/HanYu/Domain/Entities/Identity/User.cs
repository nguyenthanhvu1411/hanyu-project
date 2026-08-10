using Microsoft.AspNetCore.Identity;

namespace HanYu.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public Guid PublicId { get; private set; }
        = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? DeletedAt { get; private set; }

    public DateTimeOffset? LastLoginAt { get; private set; }

    public UserProfile? Profile { get; private set; }

    public UserPreference? Preference { get; private set; }

    public ICollection<UserSession> Sessions { get; private set; }
        = new List<UserSession>();

    public ICollection<RefreshToken> RefreshTokens { get; private set; }
        = new List<RefreshToken>();

    public ICollection<UserConsent> Consents { get; private set; }
        = new List<UserConsent>();

    public bool IsDeleted => DeletedAt.HasValue;

    protected User()
    {
    }

    public User(
        string userName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException(
                "Username không được để trống.",
                nameof(userName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email không được để trống.",
                nameof(email));

        UserName = userName.Trim();
        Email = email.Trim().ToLowerInvariant();

        NormalizedUserName =
            UserName.ToUpperInvariant();

        NormalizedEmail =
            Email.ToUpperInvariant();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "Email không được để trống.",
                nameof(email));

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 256)
            throw new ArgumentException(
                "Email không được vượt quá 256 ký tự.",
                nameof(email));

        Email = email;
        NormalizedEmail = email.ToUpperInvariant();

        EmailConfirmed = false;

        MarkUpdated();
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed)
            return;

        EmailConfirmed = true;

        MarkUpdated();
    }

    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException(
                "Username không được để trống.",
                nameof(userName));

        userName = userName.Trim();

        if (userName.Length > 256)
            throw new ArgumentException(
                "Username không được vượt quá 256 ký tự.",
                nameof(userName));

        UserName = userName;
        NormalizedUserName =
            userName.ToUpperInvariant();

        MarkUpdated();
    }

    public void UpdatePhoneNumber(
        string? phoneNumber)
    {
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber)
            ? null
            : phoneNumber.Trim();

        PhoneNumberConfirmed = false;

        MarkUpdated();
    }

    public void ConfirmPhoneNumber()
    {
        if (PhoneNumberConfirmed)
            return;

        PhoneNumberConfirmed = true;

        MarkUpdated();
    }

    public void MarkLogin()
    {
        if (IsDeleted)
            throw new InvalidOperationException(
                "Tài khoản đã bị xóa.");

        LastLoginAt = DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void EnableTwoFactor()
    {
        if (TwoFactorEnabled)
            return;

        TwoFactorEnabled = true;

        MarkUpdated();
    }

    public void DisableTwoFactor()
    {
        if (!TwoFactorEnabled)
            return;

        TwoFactorEnabled = false;

        MarkUpdated();
    }

    public void LockUntil(
        DateTimeOffset lockoutEnd)
    {
        if (lockoutEnd <= DateTimeOffset.UtcNow)
            throw new ArgumentOutOfRangeException(
                nameof(lockoutEnd));

        LockoutEnabled = true;
        LockoutEnd = lockoutEnd;

        MarkUpdated();
    }

    public void Unlock()
    {
        LockoutEnd = null;
        AccessFailedCount = 0;

        MarkUpdated();
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            return;

        DeletedAt = DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Restore()
    {
        if (!IsDeleted)
            return;

        DeletedAt = null;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
