namespace HanYu.Application.Features.Identity.Profile.UpdateProfile;

public sealed record UpdateUserProfileRequest(
    string DisplayName,
    string? AvatarUrl,
    short CurrentHskLevel,
    short DailyGoalMinutes,
    string Timezone,
    string UiLanguage);
