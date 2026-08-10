using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Entities.Analytics;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Analytics;

public sealed class LearningAnalyticsCollector
    : ILearningAnalyticsCollector
{
    private readonly HanYuDbContext _db;

    public LearningAnalyticsCollector(
        HanYuDbContext db)
    {
        _db = db;
    }

    public async Task RegisterLessonStartedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterLessonStarted();

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterLessonCompletedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterLessonCompleted();

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterReviewAsync(
        Guid userId,
        bool wasCorrect,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterVocabularyReviewed(
            wasCorrect);

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterVocabularyLearnedAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterVocabularyLearned();

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterQuizAttemptAsync(
        Guid userId,
        bool passed,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterQuizAttempt(passed);

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterAiInteractionAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.RegisterAiInteraction();

        await SaveAsync(cancellationToken);
    }

    public async Task RegisterXpAsync(
        Guid userId,
        int xp,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.AddXp(xp);

        await SaveAsync(cancellationToken);
    }

    public async Task AddLearningTimeAsync(
        Guid userId,
        int seconds,
        CancellationToken cancellationToken = default)
    {
        var stat =
            await GetTodayAsync(
                userId,
                cancellationToken);

        stat.AddLearningTime(seconds);

        await SaveAsync(cancellationToken);
    }

    private async Task<DailyLearningStat>
        GetTodayAsync(
            Guid userId,
            CancellationToken cancellationToken)
    {
        var date =
            DateOnly.FromDateTime(
                DateTime.UtcNow);

        var stat =
            await _db.Set<DailyLearningStat>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.StatDate == date,
                    cancellationToken);

        if (stat is not null)
            return stat;

        stat =
            new DailyLearningStat(
                userId,
                date);

        _db.Add(stat);

        return stat;
    }

    private Task SaveAsync(
        CancellationToken cancellationToken)
        => _db.SaveChangesAsync(
            cancellationToken);
}
