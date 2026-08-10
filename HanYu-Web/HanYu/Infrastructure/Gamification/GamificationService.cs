using HanYu.Application.Common.Models;
using HanYu.Application.Features.Gamification.Public.Achievements;
using HanYu.Application.Features.Gamification.Public.Profile;
using HanYu.Application.Features.Gamification.Public.Xp;
using HanYu.Application.Interfaces.Gamification;
using HanYu.Domain.Entities.Analytics;
using HanYu.Domain.Entities.Gamification;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Gamification;

public sealed class GamificationService : IGamificationService
{
    private readonly HanYuDbContext _db;

    public GamificationService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<GamificationProfileResponse>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var totalXp = await _db.Set<XpTransaction>()
            .Where(x => x.UserId == userId)
            .SumAsync(x => (int?)x.Amount, cancellationToken) ?? 0;

        var streak = await _db.Set<UserStreak>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        var achievements = await _db.Set<UserAchievement>()
            .CountAsync(x => x.UserId == userId, cancellationToken);

        return Result.Success(new GamificationProfileResponse(
            totalXp,
            streak?.CurrentStreak ?? 0,
            streak?.LongestStreak ?? 0,
            streak?.TotalActiveDays ?? 0,
            streak?.LastLearningDate,
            achievements));
    }

    public async Task<Result<IReadOnlyCollection<AchievementResponse>>> GetAchievementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var achievements = await _db.Set<Achievement>()
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToArrayAsync(cancellationToken);

        var unlocked = await _db.Set<UserAchievement>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToDictionaryAsync(x => x.AchievementId, cancellationToken);

        var result = achievements.Select(x =>
        {
            unlocked.TryGetValue(x.Id, out var userAchievement);

            return new AchievementResponse(
                x.PublicId,
                x.Code,
                x.NameVi,
                x.DescriptionVi,
                x.IconUrl,
                x.XpReward,
                userAchievement is not null,
                userAchievement?.UnlockedAt);
        }).ToArray();

        return Result.Success<IReadOnlyCollection<AchievementResponse>>(result);
    }

    public async Task<Result> AwardXpAsync(
        Guid userId,
        int amount,
        string reason,
        string sourceType,
        string sourceId,
        CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            return Result.Failure(Error.Validation("Gamification.InvalidXp", "XP thưởng phải lớn hơn 0."));
        }

        var exists = await _db.Set<XpTransaction>()
            .AnyAsync(x => x.UserId == userId && x.SourceType == sourceType && x.SourceId == sourceId && x.Amount > 0, cancellationToken);

        if (exists)
            return Result.Success();

        _db.Add(new XpTransaction(userId, amount, reason, sourceType, sourceId));
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RegisterLearningActivityAsync(
        Guid userId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default)
    {
        var timezone = await _db.Set<Domain.Entities.Identity.UserProfile>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Timezone)
            .FirstOrDefaultAsync(cancellationToken);

        var localDate = ResolveLocalDate(occurredAt, timezone);

        var streak = await _db.Set<UserStreak>()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (streak is null)
        {
            streak = new UserStreak(userId);
            _db.Add(streak);
        }

        var increased = streak.RegisterLearningDay(localDate);

        if (!increased)
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UnlockAchievementAsync(
        Guid userId,
        string achievementCode,
        CancellationToken cancellationToken = default)
    {
        var code = achievementCode.Trim().ToUpperInvariant();

        var achievement = await _db.Set<Achievement>()
            .FirstOrDefaultAsync(x => x.Code == code && x.IsActive, cancellationToken);

        if (achievement is null)
        {
            return Result.Failure(Error.NotFound("Achievement.NotFound", "Không tìm thấy achievement."));
        }

        var exists = await _db.Set<UserAchievement>()
            .AnyAsync(x => x.UserId == userId && x.AchievementId == achievement.Id, cancellationToken);

        if (exists)
            return Result.Success();

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        _db.Add(new UserAchievement(userId, achievement.Id));

        if (achievement.XpReward > 0)
        {
            _db.Add(new XpTransaction(
                userId,
                achievement.XpReward,
                $"Achievement: {achievement.Code}",
                "achievement",
                achievement.Id.ToString()));
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<XpTransactionResponse>>> GetXpHistoryAsync(
        Guid userId,
        XpHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _db.Set<XpTransaction>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.SourceType))
        {
            var type = query.SourceType.Trim();
            source = source.Where(x => x.SourceType == type);
        }

        source = source.OrderByDescending(x => x.CreatedAt);

        var total = await source.LongCountAsync(cancellationToken);

        var rows = await source
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        return Result.Success(new PagedResult<XpTransactionResponse>(
            rows.Select(x => new XpTransactionResponse(
                x.PublicId,
                x.Amount,
                x.Reason,
                x.SourceType,
                x.SourceId,
                x.CreatedAt)).ToArray(),
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    private static DateOnly ResolveLocalDate(DateTimeOffset occurredAt, string? timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone))
        {
            return DateOnly.FromDateTime(occurredAt.UtcDateTime);
        }

        try
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var local = TimeZoneInfo.ConvertTime(occurredAt, zone);
            return DateOnly.FromDateTime(local.DateTime);
        }
        catch
        {
            return DateOnly.FromDateTime(occurredAt.UtcDateTime);
        }
    }
}
