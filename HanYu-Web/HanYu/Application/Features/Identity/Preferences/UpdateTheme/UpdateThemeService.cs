using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.UpdateTheme;

public sealed class UpdateThemeService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public UpdateThemeService(
        IUserPreferenceService preferenceService)
    {
        _preferenceService =
            preferenceService;
    }

    public Task<Result<UserPreferenceResponse>>
        ExecuteAsync(
            Guid userId,
            UpdateThemeRequest request,
            CancellationToken cancellationToken = default)
    {
        return _preferenceService
            .UpdateThemeAsync(
                userId,
                request,
                cancellationToken);
    }
}
