using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Preferences.UpdateFlashcardMode;

public sealed class UpdateFlashcardModeService
{
    private readonly IUserPreferenceService
        _preferenceService;

    public UpdateFlashcardModeService(
        IUserPreferenceService preferenceService)
    {
        _preferenceService =
            preferenceService;
    }

    public Task<Result<UserPreferenceResponse>>
        ExecuteAsync(
            Guid userId,
            UpdateFlashcardModeRequest request,
            CancellationToken cancellationToken = default)
    {
        return _preferenceService
            .UpdateFlashcardModeAsync(
                userId,
                request,
                cancellationToken);
    }
}
