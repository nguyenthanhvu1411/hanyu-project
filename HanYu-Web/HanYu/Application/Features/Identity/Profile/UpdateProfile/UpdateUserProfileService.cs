using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Profile.UpdateProfile;

public sealed class UpdateUserProfileService
{
    private readonly IUserProfileService _profileService;

    public UpdateUserProfileService(
        IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<Result<UserProfileResponse>> ExecuteAsync(
        Guid userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        return _profileService.UpdateAsync(
            userId,
            request,
            cancellationToken);
    }
}
