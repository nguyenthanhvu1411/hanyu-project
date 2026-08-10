namespace HanYu.Application.Features.Identity.Profile.Common;

public sealed record UserProfileResponse(
    Guid PublicId,
    string UserName,
    string Email,
    bool EmailConfirmed,
    string DisplayName,
    string? AvatarUrl,
    short CurrentHskLevel,
    short DailyGoalMinutes,
    string Timezone,
    string UiLanguage,
    bool OnboardingCompleted,
    DateTimeOffset? OnboardingCompletedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
