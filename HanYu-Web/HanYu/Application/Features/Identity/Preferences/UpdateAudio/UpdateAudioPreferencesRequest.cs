namespace HanYu.Application.Features.Identity.Preferences.UpdateAudio;

public sealed record UpdateAudioPreferencesRequest(
    bool AutoPlayAudio,
    decimal AudioPlaybackRate);
