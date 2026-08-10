using HanYu.Application.Common.Models;
using HanYu.Application.Features.Analytics.Public.Me;
using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Entities.Analytics;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Analytics;

public sealed class AnalyticsPublicService : IAnalyticsPublicService
{
    private readonly HanYuDbContext _db;

    public AnalyticsPublicService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IReadOnlyCollection<MyLearningStatResponse>>>
        GetMyStatsAsync(
            Guid userId,
            DateOnly? from,
            DateOnly? to,
            CancellationToken cancellationToken = default)
    {
        var q = _db.Set<DailyLearningStat>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (from.HasValue)
            q = q.Where(x => x.StatDate >= from.Value);
            
        if (to.HasValue)
            q = q.Where(x => x.StatDate <= to.Value);

        var stats = await q
            .OrderBy(x => x.StatDate)
            .Select(x => new MyLearningStatResponse(
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
                x.XpEarned))
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<MyLearningStatResponse>>(stats);
    }

    public async Task<Result<MyLearningSummaryResponse>>
        GetMySummaryAsync(
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
