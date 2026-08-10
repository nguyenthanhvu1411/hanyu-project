using HanYu.Domain.Entities;
using HanYu.Domain.Constants;

namespace HanYu.Domain.Entities.Identity;

public class UserProfile : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; }
        = string.Empty;

    public string? AvatarUrl { get; private set; }

    public short CurrentHskLevel { get; private set; }
        = 1;

    public short DailyGoalMinutes { get; private set; }
        = 15;

    public string Timezone { get; private set; }
        = SystemConstants.DefaultTimeZone;

    public string UiLanguage { get; private set; }
        = SystemConstants.DefaultUiLanguage;

    public bool OnboardingCompleted { get; private set; }

    public DateTimeOffset? OnboardingCompletedAt { get; private set; }

    public User User { get; private set; } = null!;

    protected UserProfile()
    {
    }

    public UserProfile(
        Guid userId,
        string displayName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;

        UpdateDisplayName(displayName);
    }

    public void UpdateDisplayName(
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Tên hiển thị không được để trống.",
                nameof(displayName));

        displayName = displayName.Trim();

        if (displayName.Length > 120)
            throw new ArgumentException(
                "Tên hiển thị không được vượt quá 120 ký tự.",
                nameof(displayName));

        DisplayName = displayName;

        MarkUpdated();
    }

    public void UpdateAvatar(
        string? avatarUrl)
    {
        avatarUrl = Normalize(avatarUrl);

        if (avatarUrl?.Length > 2048)
            throw new ArgumentException(
                "Avatar URL không được vượt quá 2048 ký tự.",
                nameof(avatarUrl));

        AvatarUrl = avatarUrl;

        MarkUpdated();
    }

    public void UpdateLearningPreferences(
        short currentHskLevel,
        short dailyGoalMinutes,
        string timezone,
        string uiLanguage)
    {
        if (currentHskLevel < LearningConstants.MinHskLevel ||
            currentHskLevel > LearningConstants.MaxHskLevel)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentHskLevel),
                $"HSK level phải từ {LearningConstants.MinHskLevel} đến {LearningConstants.MaxHskLevel}.");
        }

        if (dailyGoalMinutes < LearningConstants.MinDailyGoalMinutes ||
            dailyGoalMinutes > LearningConstants.MaxDailyGoalMinutes)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyGoalMinutes),
                $"Mục tiêu học phải từ {LearningConstants.MinDailyGoalMinutes} đến {LearningConstants.MaxDailyGoalMinutes} phút/ngày.");
        }

        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException(
                "Timezone không được để trống.",
                nameof(timezone));

        if (string.IsNullOrWhiteSpace(uiLanguage))
            throw new ArgumentException(
                "Ngôn ngữ giao diện không được để trống.",
                nameof(uiLanguage));

        timezone = timezone.Trim();
        uiLanguage = uiLanguage.Trim();

        if (timezone.Length > 100)
            throw new ArgumentException(
                "Timezone quá dài.",
                nameof(timezone));

        if (uiLanguage.Length > 10)
            throw new ArgumentException(
                "UiLanguage quá dài.",
                nameof(uiLanguage));

        CurrentHskLevel = currentHskLevel;
        DailyGoalMinutes = dailyGoalMinutes;
        Timezone = timezone;
        UiLanguage = uiLanguage;

        MarkUpdated();
    }

    public void CompleteOnboarding()
    {
        if (OnboardingCompleted)
            return;

        OnboardingCompleted = true;
        OnboardingCompletedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void ResetOnboarding()
    {
        if (!OnboardingCompleted &&
            !OnboardingCompletedAt.HasValue)
        {
            return;
        }

        OnboardingCompleted = false;
        OnboardingCompletedAt = null;

        MarkUpdated();
    }

    private static string? Normalize(
        string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
