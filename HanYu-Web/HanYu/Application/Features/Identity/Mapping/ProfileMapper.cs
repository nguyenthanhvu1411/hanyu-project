using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Features.Identity.Mapping;

public static class ProfileMapper
{
    public static UserProfileResponse ToUserProfileResponse(User user)
    {
        var profile = user.Profile ?? throw new InvalidOperationException("User profile is null");

        return new UserProfileResponse(
            user.PublicId,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            profile.DisplayName,
            profile.AvatarUrl,
            profile.CurrentHskLevel,
            profile.DailyGoalMinutes,
            profile.Timezone,
            profile.UiLanguage,
            profile.OnboardingCompleted,
            profile.OnboardingCompletedAt,
            profile.CreatedAt,
            profile.UpdatedAt);
    }
}
