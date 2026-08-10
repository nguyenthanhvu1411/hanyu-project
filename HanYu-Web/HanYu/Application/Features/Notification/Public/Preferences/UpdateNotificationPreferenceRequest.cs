namespace HanYu.Application.Features.Notification.Public.Preferences;

public sealed record UpdateNotificationPreferenceRequest(
    bool InAppEnabled,
    bool EmailEnabled,
    bool LearningReminderEnabled,
    bool ReviewReminderEnabled,
    bool SecurityNotificationEnabled,
    TimeOnly? PreferredReminderTime,
    string Timezone);
