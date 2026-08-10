using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Domain.Entities.Identity;

namespace HanYu.Application.Features.Identity.Mapping;

public static class PreferenceMapper
{
    public static UserPreferenceResponse ToUserPreferenceResponse(UserPreference preference)
    {
        return new UserPreferenceResponse(
            preference.ShowPinyin,
            preference.ShowTraditional,
            preference.AutoPlayAudio,
            preference.AudioPlaybackRate,
            preference.Theme,
            preference.DefaultFlashcardMode,
            preference.ReducedMotion,
            preference.CreatedAt,
            preference.UpdatedAt);
    }
}
