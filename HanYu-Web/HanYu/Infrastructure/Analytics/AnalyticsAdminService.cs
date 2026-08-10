using HanYu.Application.Common.Models;
using HanYu.Application.Features.Analytics.Admin.Dashboard;
using HanYu.Application.Features.Analytics.Admin.Users;
using HanYu.Application.Features.Analytics.Public.Me;
using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Entities.Analytics;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Analytics;

public sealed class AnalyticsAdminService : IAnalyticsAdminService
{
    private readonly HanYuDbContext _db;

    public AnalyticsAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<AdminAnalyticsDashboardResponse>>
        GetDashboardAsync(
            CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var q = _db.Set<DailyLearningStat>()
            .AsNoTracking()
            .Where(x => x.StatDate == today);

        var activeUsers = await q.Select(x => x.UserId).Distinct().CountAsync(cancellationToken);
        
        var learningSeconds = await q.SumAsync(x => (long)x.LearningSeconds, cancellationToken);
        var lessonsCompleted = await q.SumAsync(x => (long)x.LessonsCompleted, cancellationToken);
        var vocabularyReviewed = await q.SumAsync(x => (long)x.VocabularyReviewed, cancellationToken);
        var quizAttempts = await q.SumAsync(x => (long)x.QuizAttempts, cancellationToken);
        var quizPassed = await q.SumAsync(x => (long)x.QuizPassed, cancellationToken);
        var aiInteractions = await q.SumAsync(x => (long)x.AiInteractions, cancellationToken);
        var xpEarned = await q.SumAsync(x => (long)x.XpEarned, cancellationToken);

        return Result.Success(new AdminAnalyticsDashboardResponse(
            activeUsers,
            learningSeconds,
            lessonsCompleted,
            vocabularyReviewed,
            quizAttempts,
            quizPassed,
            aiInteractions,
            xpEarned));
    }

    public async Task<Result<PagedResult<AdminDailyLearningStatResponse>>>
        GetDailyStatsAsync(
            AdminLearningStatQuery query,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<DailyLearningStat>().AsNoTracking();

        if (query.UserId.HasValue)
            q = q.Where(x => x.UserId == query.UserId.Value);

        if (query.From.HasValue)
            q = q.Where(x => x.StatDate >= query.From.Value);

        if (query.To.HasValue)
            q = q.Where(x => x.StatDate <= query.To.Value);

        q = query.Sort switch
        {
            "date" => q.OrderBy(x => x.StatDate),
            "-date" => q.OrderByDescending(x => x.StatDate),
            _ => q.OrderByDescending(x => x.StatDate)
        };

        var total = await q.CountAsync(cancellationToken);

        var entities = await q
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var responses = entities.Select(x => new AdminDailyLearningStatResponse(
            x.UserId,
            x.StatDate,
            x.LearningSeconds,
            x.LessonsStarted,
            x.LessonsCompleted,
            x.VocabularyReviewed,
            x.VocabularyLearned,
            x.CorrectReviews,
            x.WrongReviews,
            x.QuizAttempts,
            x.QuizPassed,
            x.AiInteractions,
            x.XpEarned,
            x.UpdatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminDailyLearningStatResponse>(
            responses,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<MyLearningSummaryResponse>>
        GetUserSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<DailyLearningStat>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        var totalLearningSeconds = await q.SumAsync(x => x.LearningSeconds, cancellationToken);
        var lessonsCompleted = await q.SumAsync(x => x.LessonsCompleted, cancellationToken);
        var vocabularyReviewed = await q.SumAsync(x => x.VocabularyReviewed, cancellationToken);
        var vocabularyLearned = await q.SumAsync(x => x.VocabularyLearned, cancellationToken);
        var correctReviews = await q.SumAsync(x => x.CorrectReviews, cancellationToken);
        var wrongReviews = await q.SumAsync(x => x.WrongReviews, cancellationToken);
        var quizAttempts = await q.SumAsync(x => x.QuizAttempts, cancellationToken);
        var quizPassed = await q.SumAsync(x => x.QuizPassed, cancellationToken);
        var aiInteractions = await q.SumAsync(x => x.AiInteractions, cancellationToken);
        var xpEarned = await q.SumAsync(x => x.XpEarned, cancellationToken);

        var totalReviews = correctReviews + wrongReviews;
        var accuracy = totalReviews > 0 ? (decimal)correctReviews / totalReviews : 0m;

        var streak = await _db.Set<UserStreak>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return Result.Success(new MyLearningSummaryResponse(
            totalLearningSeconds,
            lessonsCompleted,
            vocabularyReviewed,
            vocabularyLearned,
            quizAttempts,
            quizPassed,
            aiInteractions,
            xpEarned,
            accuracy,
            streak?.CurrentStreak ?? 0,
            streak?.LongestStreak ?? 0,
            streak?.TotalActiveDays ?? 0,
            streak?.LastLearningDate));
    }
}
