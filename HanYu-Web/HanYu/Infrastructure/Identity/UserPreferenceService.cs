using HanYu.Application.Common.Models;
using HanYu.Application.Features.Identity.Preferences.Common;
using HanYu.Application.Features.Identity.Preferences.UpdateAudio;
using HanYu.Application.Features.Identity.Preferences.UpdateDisplay;
using HanYu.Application.Features.Identity.Preferences.UpdateFlashcardMode;
using HanYu.Application.Features.Identity.Preferences.UpdateTheme;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Identity;

public sealed class UserPreferenceService
    : IUserPreferenceService
{
    private readonly HanYuDbContext _dbContext;

    public UserPreferenceService(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<UserPreferenceResponse>>
        GetAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .AsNoTracking()
                .Include(x => x.Preference)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (user is null)
        {
            return UserNotFound();
        }

        if (user.Preference is null)
        {
            return Result.Failure<UserPreferenceResponse>(
                Error.NotFound(
                    "Identity.PreferenceNotFound",
                    "Không tìm thấy cấu hình người dùng."));
        }

        return Result.Success(
            Map(user.Preference));
    }

    public async Task<Result<UserPreferenceResponse>>
        UpdateDisplayAsync(
            Guid userId,
            UpdateDisplayPreferencesRequest request,
            CancellationToken cancellationToken = default)
    {
        var result =
            await GetTrackedPreferenceAsync(
                userId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<UserPreferenceResponse>(
                result.Error);
        }

        var preference = result.Value;

        preference.UpdateDisplayPreferences(
            request.ShowPinyin,
            request.ShowTraditional,
            request.ReducedMotion);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(preference));
    }

    public async Task<Result<UserPreferenceResponse>>
        UpdateAudioAsync(
            Guid userId,
            UpdateAudioPreferencesRequest request,
            CancellationToken cancellationToken = default)
    {
        var result =
            await GetTrackedPreferenceAsync(
                userId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<UserPreferenceResponse>(
                result.Error);
        }

        var preference = result.Value;

        try
        {
            preference.UpdateAudioPreferences(
                request.AutoPlayAudio,
                request.AudioPlaybackRate);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return Result.Failure<UserPreferenceResponse>(
                Error.Validation(
                    "Identity.InvalidAudioPreference",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(preference));
    }

    public async Task<Result<UserPreferenceResponse>>
        UpdateThemeAsync(
            Guid userId,
            UpdateThemeRequest request,
            CancellationToken cancellationToken = default)
    {
        var result =
            await GetTrackedPreferenceAsync(
                userId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<UserPreferenceResponse>(
                result.Error);
        }

        var preference = result.Value;

        try
        {
            preference.UpdateTheme(
                request.Theme);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<UserPreferenceResponse>(
                Error.Validation(
                    "Identity.InvalidTheme",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(preference));
    }

    public async Task<Result<UserPreferenceResponse>>
        UpdateFlashcardModeAsync(
            Guid userId,
            UpdateFlashcardModeRequest request,
            CancellationToken cancellationToken = default)
    {
        var result =
            await GetTrackedPreferenceAsync(
                userId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<UserPreferenceResponse>(
                result.Error);
        }

        var preference = result.Value;

        try
        {
            preference.UpdateFlashcardMode(
                request.Mode);
        }
        catch (ArgumentException exception)
        {
            return Result.Failure<UserPreferenceResponse>(
                Error.Validation(
                    "Identity.InvalidFlashcardMode",
                    exception.Message));
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(preference));
    }

    public async Task<Result<UserPreferenceResponse>>
        ResetAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var result =
            await GetTrackedPreferenceAsync(
                userId,
                cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<UserPreferenceResponse>(
                result.Error);
        }

        var preference = result.Value;

        preference.ResetToDefault();

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            Map(preference));
    }

    private async Task<Result<UserPreference>>
        GetTrackedPreferenceAsync(
            Guid userId,
            CancellationToken cancellationToken)
    {
        var user =
            await _dbContext
                .Set<User>()
                .Include(x => x.Preference)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == userId &&
                        x.DeletedAt == null,
                    cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserPreference>(
                Error.NotFound(
                    "Identity.UserNotFound",
                    "Không tìm thấy người dùng."));
        }

        // Fallback cho dữ liệu cũ.
        if (user.Preference is null)
        {
            var preference =
                new UserPreference(user.Id);

            _dbContext
                .Set<UserPreference>()
                .Add(preference);

            return Result.Success(
                preference);
        }

        return Result.Success(
            user.Preference);
    }

    private static UserPreferenceResponse Map(
        UserPreference preference)
    {
        return new UserPreferenceResponse(
            preference.ShowPinyin,
            preference.ShowTraditional,
            preference.AutoPlayAudio,
            preference.AudioPlaybackRate,
            preference.Theme,
            preference.DefaultFlashcardMode,
            preference.ReducedMotion,
            preference.CreatedAt,
            preference.UpdatedAt);
    }

    private static Result<UserPreferenceResponse>
        UserNotFound()
    {
        return Result.Failure<UserPreferenceResponse>(
            Error.NotFound(
                "Identity.UserNotFound",
                "Không tìm thấy người dùng."));
    }
}
