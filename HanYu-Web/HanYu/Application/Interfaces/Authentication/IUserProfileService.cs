using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Application.Features.Identity.Profile.UpdateProfile;

namespace HanYu.Application.Interfaces.Authentication;

public interface IUserProfileService
{
    Task<Result<UserProfileResponse>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfileResponse>> UpdateAsync(
        Guid userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<UserProfileResponse>> CompleteOnboardingAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
