using System.IO.Compression;
using System.Text.Json;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.BackgroundJobs.DataExport;

public sealed class IdentityDataExportBuilder
    : IIdentityDataExportBuilder
{
    private readonly HanYuDbContext _dbContext;

    private static readonly JsonSerializerOptions
        JsonOptions =
            new()
            {
                WriteIndented = true
            };

    public IdentityDataExportBuilder(
        HanYuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stream> BuildAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user =
            await _dbContext
                .Set<User>()
                .AsNoTracking()
                .Include(x => x.Profile)
                .Include(x => x.Preference)
                .FirstOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken)
            ?? throw new InvalidOperationException(
                "User không tồn tại.");

        var consents =
            await _dbContext
                .Set<UserConsent>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.ConsentType)
                .Select(x => new
                {
                    x.ConsentType,
                    x.Version,
                    x.IsGranted,
                    x.GrantedAt,
                    x.RevokedAt
                })
                .ToListAsync(
                    cancellationToken);

        var sessions =
            await _dbContext
                .Set<UserSession>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(
                    x => x.LastActivityAt)
                .Select(x => new
                {
                    x.SessionKey,
                    x.DeviceName,
                    x.DeviceType,
                    x.Browser,
                    x.OperatingSystem,
                    x.IpAddress,
                    x.LastActivityAt,
                    x.RevokedAt,
                    x.Status,
                    x.CreatedAt,
                    x.UpdatedAt
                })
                .ToListAsync(
                    cancellationToken);

        var securityEvents =
            await _dbContext
                .Set<UserSecurityEvent>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(
                    x => x.OccurredAt)
                .Select(x => new
                {
                    x.EventType,
                    x.IpAddress,
                    x.UserAgent,
                    x.MetadataJson,
                    x.OccurredAt
                })
                .ToListAsync(
                    cancellationToken);

        var loginHistory =
            await _dbContext
                .Set<UserLoginHistory>()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(
                    x => x.AttemptedAt)
                .Select(x => new
                {
                    x.IsSuccessful,
                    x.IpAddress,
                    x.UserAgent,
                    x.DeviceName,
                    x.Browser,
                    x.OperatingSystem,
                    x.FailureReason,
                    x.AttemptedAt
                })
                .ToListAsync(
                    cancellationToken);

        var stream = new MemoryStream();

        using (
            var archive =
                new ZipArchive(
                    stream,
                    ZipArchiveMode.Create,
                    leaveOpen: true))
        {
            await WriteJsonAsync(
                archive,
                "account.json",
                new
                {
                    user.PublicId,
                    user.UserName,
                    user.Email,
                    user.EmailConfirmed,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    user.TwoFactorEnabled,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.LastLoginAt
                },
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "profile.json",
                user.Profile is null
                    ? null
                    : new
                    {
                        user.Profile.DisplayName,
                        user.Profile.AvatarUrl,
                        user.Profile.CurrentHskLevel,
                        user.Profile.DailyGoalMinutes,
                        user.Profile.Timezone,
                        user.Profile.UiLanguage,
                        user.Profile.OnboardingCompleted,
                        user.Profile.OnboardingCompletedAt
                    },
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "preferences.json",
                user.Preference is null
                    ? null
                    : new
                    {
                        user.Preference.ShowPinyin,
                        user.Preference.ShowTraditional,
                        user.Preference.AutoPlayAudio,
                        user.Preference.AudioPlaybackRate,
                        user.Preference.Theme,
                        user.Preference.DefaultFlashcardMode,
                        user.Preference.ReducedMotion
                    },
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "consents.json",
                consents,
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "sessions.json",
                sessions,
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "security-events.json",
                securityEvents,
                cancellationToken);

            await WriteJsonAsync(
                archive,
                "login-history.json",
                loginHistory,
                cancellationToken);
        }

        stream.Position = 0;

        return stream;
    }

    private static async Task WriteJsonAsync<T>(
        ZipArchive archive,
        string fileName,
        T value,
        CancellationToken cancellationToken)
    {
        var entry =
            archive.CreateEntry(
                fileName,
                CompressionLevel.Optimal);

        await using var stream =
            entry.Open();

        await JsonSerializer.SerializeAsync(
            stream,
            value,
            JsonOptions,
            cancellationToken);
    }
}
