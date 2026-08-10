using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Profile.Onboarding;

public sealed class CompleteOnboardingService
{
    private readonly IUserProfileService _profileService;

    public CompleteOnboardingService(
        IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<Result<UserProfileResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _profileService.CompleteOnboardingAsync(
            userId,
            cancellationToken);
    }
}
