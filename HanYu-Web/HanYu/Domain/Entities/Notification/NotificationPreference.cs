namespace HanYu.Domain.Entities.Notification;

public class NotificationPreference
{
    public Guid UserId { get; private set; }

    public bool InAppEnabled { get; private set; }
        = true;

    public bool EmailEnabled { get; private set; }
        = true;

    public bool LearningReminderEnabled { get; private set; }
        = true;

    public bool ReviewReminderEnabled { get; private set; }
        = true;

    public bool SecurityNotificationEnabled { get; private set; }
        = true;

    public TimeOnly? PreferredReminderTime { get; private set; }

    public string Timezone { get; private set; }
        = "Asia/Ho_Chi_Minh";

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected NotificationPreference()
    {
    }

    public NotificationPreference(
        Guid userId,
        string timezone = "Asia/Ho_Chi_Minh")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException(
                "Timezone không được để trống.",
                nameof(timezone));

        UserId = userId;
        Timezone = timezone.Trim();
    }

    public void UpdateChannels(
        bool inAppEnabled,
        bool emailEnabled)
    {
        InAppEnabled = inAppEnabled;
        EmailEnabled = emailEnabled;

        MarkUpdated();
    }

    public void UpdateLearningReminder(
        bool enabled,
        TimeOnly? preferredReminderTime,
        string timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException(
                "Timezone không được để trống.",
                nameof(timezone));

        LearningReminderEnabled = enabled;
        PreferredReminderTime = preferredReminderTime;
        Timezone = timezone.Trim();

        MarkUpdated();
    }

    public void UpdateReviewReminder(
        bool enabled)
    {
        ReviewReminderEnabled = enabled;

        MarkUpdated();
    }

    public void UpdateSecurityNotifications(
        bool enabled)
    {
        SecurityNotificationEnabled = enabled;

        MarkUpdated();
    }

    public void EnableAll()
    {
        InAppEnabled = true;
        EmailEnabled = true;
        LearningReminderEnabled = true;
        ReviewReminderEnabled = true;
        SecurityNotificationEnabled = true;

        MarkUpdated();
    }

    public void DisableNonSecurity()
    {
        InAppEnabled = false;
        EmailEnabled = false;
        LearningReminderEnabled = false;
        ReviewReminderEnabled = false;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt =
            DateTimeOffset.UtcNow;
    }
}
