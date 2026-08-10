using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.GetPreferences;

public sealed class GetUserPreferencesService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public GetUserPreferencesService(
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
        return _preferenceService.GetAsync(
            userId,
            cancellationToken);
    }
}
