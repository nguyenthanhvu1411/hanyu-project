namespace HanYu.Application.Features.Identity.Preferences.UpdateDisplay;

public sealed record UpdateDisplayPreferencesRequest(
    bool ShowPinyin,
    bool ShowTraditional,
    bool ReducedMotion);
