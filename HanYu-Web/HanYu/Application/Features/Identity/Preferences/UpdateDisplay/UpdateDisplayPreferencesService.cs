using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.UpdateDisplay;

public sealed class UpdateDisplayPreferencesService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public UpdateDisplayPreferencesService(
        IUserPreferenceService preferenceService)
    {
        _preferenceService =
            preferenceService;
    }

    public Task<Result<UserPreferenceResponse>>
        ExecuteAsync(
            Guid userId,
            UpdateDisplayPreferencesRequest request,
            CancellationToken cancellationToken = default)
    {
        return _preferenceService
            .UpdateDisplayAsync(
                userId,
                request,
                cancellationToken);
    }
}
