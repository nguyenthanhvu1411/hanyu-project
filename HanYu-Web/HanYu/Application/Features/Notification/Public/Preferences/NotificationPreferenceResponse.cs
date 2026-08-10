namespace HanYu.Application.Features.Notification.Public.Preferences;

public sealed record NotificationPreferenceResponse(
    bool InAppEnabled,
    bool EmailEnabled,
    bool LearningReminderEnabled,
    bool ReviewReminderEnabled,
    bool SecurityNotificationEnabled,
    TimeOnly? PreferredReminderTime,
    string Timezone,
    DateTimeOffset UpdatedAt);
