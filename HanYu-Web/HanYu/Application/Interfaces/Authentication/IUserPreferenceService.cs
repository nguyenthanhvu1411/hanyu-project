using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Features.Identity.Preferences.UpdateAudio;
using HanYu.Application.Features.Identity.Preferences.UpdateDisplay;
using HanYu.Application.Features.Identity.Preferences.UpdateFlashcardMode;
using HanYu.Application.Features.Identity.Preferences.UpdateTheme;

namespace HanYu.Application.Interfaces.Authentication;

public interface IUserPreferenceService
{
    Task<Result<UserPreferenceResponse>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<UserPreferenceResponse>>
        UpdateDisplayAsync(
            Guid userId,
            UpdateDisplayPreferencesRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<UserPreferenceResponse>>
        UpdateAudioAsync(
            Guid userId,
            UpdateAudioPreferencesRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<UserPreferenceResponse>>
        UpdateThemeAsync(
            Guid userId,
            UpdateThemeRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<UserPreferenceResponse>>
        UpdateFlashcardModeAsync(
            Guid userId,
            UpdateFlashcardModeRequest request,
            CancellationToken cancellationToken = default);

    Task<Result<UserPreferenceResponse>>
        ResetAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
}
