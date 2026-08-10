using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Application.Interfaces.Authentication;

namespace HanYu.Application.Features.Identity.Profile.GetProfile;

public sealed class GetUserProfileService
{
    private readonly IUserProfileService _profileService;

    public GetUserProfileService(
        IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<Result<UserProfileResponse>> ExecuteAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _profileService.GetAsync(
            userId,
            cancellationToken);
    }
}
