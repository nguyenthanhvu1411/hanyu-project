using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Profile.Common;
using HanYu.Application.Features.Identity.Profile.UpdateProfile;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class UserProfileService
    : IUserProfileService
{
    private readonly HanYuDbContext _dbContext;

    public UserProfileService(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserProfileResponse>> GetAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .AsNoTracking()
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.Profile is null)
        {
            return Result.Failure<UserProfileResponse>(
                Error.NotFound(
                    "Identity.ProfileNotFound",
                    "Không tìm thấy hồ sơ người dùng."));
        }

        return Result.Success(
            Map(
                user,
                user.Profile));
    }

    public async Task<Result<UserProfileResponse>> UpdateAsync(
        Guid userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        var profile = user.Profile;

        // Phòng trường hợp dữ liệu cũ chưa có profile.
        if (profile is null)
        {
            profile =
                new UserProfile(
                    user.Id,
                    request.DisplayName);

            _dbContext
                .Set<UserProfile>()
                .Add(profile);
        }

        try
        {
            profile.UpdateDisplayName(
                request.DisplayName);

            profile.UpdateAvatar(
                request.AvatarUrl);

            profile.UpdateLearningPreferences(
                request.CurrentHskLevel,
                request.DailyGoalMinutes,
                request.Timezone,
                request.UiLanguage);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<UserProfileResponse>(
                Error.Validation(
                    "Identity.InvalidProfile",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(
                user,
                profile));
    }

    public async Task<Result<UserProfileResponse>> CompleteOnboardingAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        if (user.Profile is null)
        {
            return Result.Failure<UserProfileResponse>(
                Error.NotFound(
                    "Identity.ProfileNotFound",
                    "Không tìm thấy hồ sơ người dùng."));
        }

        user.Profile.CompleteOnboarding();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(
                user,
                user.Profile));
    }

    private static UserProfileResponse Map(
        User user,
        UserProfile profile)
    {
        return new UserProfileResponse(
            user.PublicId,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.EmailConfirmed,
            profile.DisplayName,
            profile.AvatarUrl,
            profile.CurrentHskLevel,
            profile.DailyGoalMinutes,
            profile.Timezone,
            profile.UiLanguage,
            profile.OnboardingCompleted,
            profile.OnboardingCompletedAt,
            profile.CreatedAt,
            profile.UpdatedAt);
    }
}
