using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.ResetPreferences;

public sealed class ResetUserPreferencesService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public ResetUserPreferencesService(
        IUserPreferenceService preferenceService)
    {
        _preferenceService =
            preferenceService;
    }

    public Task<Result<UserPreferenceResponse>>
        ExecuteAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        return _preferenceService.ResetAsync(
            userId,
            cancellationToken);
    }
}
