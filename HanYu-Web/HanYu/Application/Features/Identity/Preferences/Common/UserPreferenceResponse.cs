namespace HanYu.Application.Features.Identity.Preferences.Common;

public sealed record UserPreferenceResponse(
    bool ShowPinyin,
    bool ShowTraditional,
    bool AutoPlayAudio,
    decimal AudioPlaybackRate,
    string Theme,
    string DefaultFlashcardMode,
    bool ReducedMotion,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
