using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Mapping;
using HanYu.Application.Features.Review.Public.Favorites;
using HanYu.Application.Features.Review.Public.Queue;
using HanYu.Application.Features.Review.Public.Review;
using HanYu.Application.Interfaces.Review;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Review;

public sealed class ReviewService : IReviewService
{
    private readonly HanYuDbContext _db;
    private readonly IReviewScheduler _scheduler;

    public ReviewService(
        HanYuDbContext db,
        IReviewScheduler scheduler)
    {
        _db = db;
        _scheduler = scheduler;
    }

    public async Task<Result<
        IReadOnlyCollection<ReviewQueueItemResponse>>>
        GetQueueAsync(
            Guid userId,
            ReviewQueueQuery query,
            CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure<
                IReadOnlyCollection<ReviewQueueItemResponse>>(
                Error.Unauthorized(
                    "Identity.Unauthorized",
                    "Người dùng chưa đăng nhập."));
        }

        var limit =
            Math.Clamp(
                query.Limit,
                1,
                ReviewConstants.MaxReviewQueueSize);

        var now =
            DateTimeOffset.UtcNow;

        var source =
            _db.Set<UserVocabularyState>()
                .AsNoTracking()
                .Include(x => x.Vocabulary)
                    .ThenInclude(x => x.Topic)
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.Vocabulary.Status ==
                            ContentStatus.Published &&
                        (
                            x.NextReviewAt == null ||
                            x.NextReviewAt <= now
                        ));

        if (query.HskLevel.HasValue)
        {
            source =
                source.Where(
                    x =>
                        x.Vocabulary.HskLevelId ==
                        query.HskLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                query.Topic))
        {
            var topic =
                query.Topic.Trim()
                    .ToLowerInvariant();

            source =
                source.Where(
                    x =>
                        x.Vocabulary.Topic != null &&
                        x.Vocabulary.Topic.Slug ==
                            topic);
        }

        var states =
            await source
                .OrderBy(
                    x => x.NextReviewAt ?? DateTimeOffset.MinValue)
                .ThenBy(x => x.MasteryScore)
                .Take(limit)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<ReviewQueueItemResponse>>(
            states
                .Select(ReviewMapper.ToQueueItem)
                .ToArray());
    }

    public async Task<Result<ReviewQueueSummaryResponse>>
        GetSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var now =
            DateTimeOffset.UtcNow;

        var source =
            _db.Set<UserVocabularyState>()
                .AsNoTracking()
                .Where(
                    x => x.UserId == userId);

        var dueCount =
            await source.CountAsync(
                x =>
                    x.NextReviewAt == null ||
                    x.NextReviewAt <= now,
                cancellationToken);

        var newCount =
            await source.CountAsync(
                x =>
                    x.LearningState ==
                    LearningState.NotStarted,
                cancellationToken);

        var learningCount =
            await source.CountAsync(
                x =>
                    x.LearningState ==
                    LearningState.Learning,
                cancellationToken);

        var knownCount =
            await source.CountAsync(
                x =>
                    x.LearningState ==
                    LearningState.Known,
                cancellationToken);

        var masteredCount =
            await source.CountAsync(
                x =>
                    x.LearningState ==
                    LearningState.Mastered,
                cancellationToken);

        return Result.Success(
            new ReviewQueueSummaryResponse(
                dueCount,
                newCount,
                learningCount,
                knownCount,
                masteredCount));
    }

    public async Task<Result<
        VocabularyLearningStateResponse>>
        GetStateAsync(
            Guid userId,
            Guid vocabularyPublicId,
            CancellationToken cancellationToken = default)
    {
        var state =
            await _db.Set<UserVocabularyState>()
                .AsNoTracking()
                .Include(x => x.Vocabulary)
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.Vocabulary.PublicId ==
                            vocabularyPublicId,
                    cancellationToken);

        if (state is null)
        {
            return Result.Failure<
                VocabularyLearningStateResponse>(
                Error.NotFound(
                    "Review.StateNotFound",
                    "Chưa có trạng thái học cho vocabulary."));
        }

        return Result.Success(
            ReviewMapper.ToStateResponse(
                state));
    }

    public async Task<Result<ReviewResultResponse>>
        SubmitReviewAsync(
            Guid userId,
            SubmitReviewRequest request,
            CancellationToken cancellationToken = default)
    {
        var vocabulary =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId ==
                            request.VocabularyPublicId &&
                        x.Status ==
                            ContentStatus.Published,
                    cancellationToken);

        if (vocabulary is null)
        {
            return Result.Failure<
                ReviewResultResponse>(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy vocabulary."));
        }

        var state =
            await _db.Set<UserVocabularyState>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            vocabulary.Id,
                    cancellationToken);

        if (state is null)
        {
            state =
                new UserVocabularyState(
                    userId,
                    vocabulary.Id);

            _db.Add(state);
        }

        var reviewedAt =
            DateTimeOffset.UtcNow;

        var schedule =
            _scheduler.Calculate(
                state.MasteryScore,
                state.CurrentIntervalMinutes,
                request.Rating,
                request.WasCorrect,
                reviewedAt);

        var localLearningDate =
            DateOnly.FromDateTime(
                reviewedAt.UtcDateTime);

        state.ApplyReview(
            request.Rating,
            request.WasCorrect,
            schedule.MasteryAfter,
            schedule.IntervalAfterMinutes,
            schedule.ReviewedAt,
            schedule.NextReviewAt,
            localLearningDate);

        var reviewEvent =
            new ReviewEvent(
                userId,
                vocabulary.Id,
                request.Rating,
                request.WasCorrect,
                schedule.MasteryBefore,
                schedule.MasteryAfter,
                schedule.IntervalAfterMinutes,
                schedule.IntervalBeforeMinutes,
                request.ResponseTimeMs);

        _db.Add(reviewEvent);

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new ReviewResultResponse(
                vocabulary.PublicId,
                request.Rating,
                request.WasCorrect,
                schedule.MasteryBefore,
                schedule.MasteryAfter,
                schedule.IntervalBeforeMinutes,
                schedule.IntervalAfterMinutes,
                schedule.NextReviewAt,
                state.LearningState));
    }

    public async Task<Result> FavoriteAsync(
        Guid userId,
        Guid vocabularyPublicId,
        CancellationToken cancellationToken = default)
    {
        var stateResult =
            await GetOrCreateStateAsync(
                userId,
                vocabularyPublicId,
                cancellationToken);

        if (stateResult.IsFailure)
            return Result.Failure(
                stateResult.Error);

        stateResult.Value.MarkFavorite();

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UnfavoriteAsync(
        Guid userId,
        Guid vocabularyPublicId,
        CancellationToken cancellationToken = default)
    {
        var state =
            await _db.Set<UserVocabularyState>()
                .Include(x => x.Vocabulary)
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.Vocabulary.PublicId ==
                            vocabularyPublicId,
                    cancellationToken);

        if (state is null)
            return Result.Success();

        state.Unfavorite();

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result<
        IReadOnlyCollection<FavoriteVocabularyResponse>>>
        GetFavoritesAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
    {
        var states =
            await _db.Set<UserVocabularyState>()
                .AsNoTracking()
                .Include(x => x.Vocabulary)
                .Where(
                    x =>
                        x.UserId == userId &&
                        x.IsFavorite &&
                        x.Vocabulary.Status ==
                            ContentStatus.Published)
                .OrderBy(
                    x => x.Vocabulary.Simplified)
                .ToArrayAsync(
                    cancellationToken);

        return Result.Success<
            IReadOnlyCollection<FavoriteVocabularyResponse>>(
            states
                .Select(
                    ReviewMapper.ToFavoriteResponse)
                .ToArray());
    }

    private async Task<Result<UserVocabularyState>>
        GetOrCreateStateAsync(
            Guid userId,
            Guid vocabularyPublicId,
            CancellationToken cancellationToken)
    {
        var vocabulary =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId ==
                            vocabularyPublicId &&
                        x.Status ==
                            ContentStatus.Published,
                    cancellationToken);

        if (vocabulary is null)
        {
            return Result.Failure<UserVocabularyState>(
                Error.NotFound(
                    "Vocabulary.NotFound",
                    "Không tìm thấy vocabulary."));
        }

        var state =
            await _db.Set<UserVocabularyState>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            vocabulary.Id,
                    cancellationToken);

        if (state is not null)
            return Result.Success(state);

        state =
            new UserVocabularyState(
                userId,
                vocabulary.Id);

        _db.Add(state);

        return Result.Success(state);
    }
}
