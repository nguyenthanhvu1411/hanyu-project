using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.UpdateAudio;

public sealed class UpdateAudioPreferencesService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public UpdateAudioPreferencesService(
        IUserPreferenceService preferenceService)
    {
        _preferenceService =
            preferenceService;
    }

    public Task<Result<UserPreferenceResponse>>
        ExecuteAsync(
            Guid userId,
            UpdateAudioPreferencesRequest request,
            CancellationToken cancellationToken = default)
    {
        return _preferenceService
            .UpdateAudioAsync(
                userId,
                request,
                cancellationToken);
    }
}
