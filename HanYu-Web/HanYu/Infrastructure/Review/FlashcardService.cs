using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Public.Flashcards;
using HanYu.Application.Features.Review.Public.Review;
using HanYu.Application.Interfaces.Review;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;

using HanYu.Application.Interfaces.Gamification;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Review;

public sealed class FlashcardService
    : IFlashcardService
{
    private readonly HanYuDbContext _db;
    private readonly IReviewScheduler _scheduler;
    private readonly IGamificationService _gamification;
    private readonly IAchievementEvaluator _achievementEvaluator;

    public FlashcardService(
        HanYuDbContext db,
        IReviewScheduler scheduler,
        IGamificationService gamification,
        IAchievementEvaluator achievementEvaluator)
    {
        _db = db;
        _scheduler = scheduler;
        _gamification = gamification;
        _achievementEvaluator = achievementEvaluator;
    }

    public async Task<Result<FlashcardSessionResponse>>
        CreateSessionAsync(
            Guid userId,
            CreateFlashcardSessionRequest request,
            CancellationToken cancellationToken = default)
    {
        var limit =
            Math.Clamp(
                request.Limit,
                1,
                ReviewConstants.MaxFlashcardSize);

        var vocabularyIdsResult =
            await ResolveVocabularyIdsAsync(
                userId,
                request,
                limit,
                cancellationToken);

        if (vocabularyIdsResult.IsFailure)
        {
            return Result.Failure<
                FlashcardSessionResponse>(
                vocabularyIdsResult.Error);
        }

        var vocabularyIds =
            vocabularyIdsResult.Value;

        if (vocabularyIds.Count == 0)
        {
            return Result.Failure<
                FlashcardSessionResponse>(
                Error.NotFound(
                    "Flashcard.NoVocabulary",
                    "Không có vocabulary phù hợp để tạo flashcard session."));
        }

        long? sourceId =
            null;

        if (request.SourceType ==
                FlashcardSourceType.Lesson &&
            request.SourcePublicId.HasValue)
        {
            sourceId =
                await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.PublicId ==
                            request.SourcePublicId.Value)
                    .Select(x => (long?)x.Id)
                    .FirstOrDefaultAsync(
                        cancellationToken);
        }
        else if (
            request.SourceType ==
                FlashcardSourceType.Topic &&
            request.SourcePublicId.HasValue)
        {
            sourceId =
                await _db.Set<Topic>()
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.PublicId ==
                            request.SourcePublicId.Value)
                    .Select(x => (long?)x.Id)
                    .FirstOrDefaultAsync(
                        cancellationToken);
        }

        await using var tx =
            await _db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var session =
                new FlashcardSession(
                    userId,
                    request.Mode,
                    request.SourceType,
                    vocabularyIds.Count,
                    sourceId);

            _db.Add(session);

            /*
             * SessionItem constructor cần FlashcardSessionId,
             * nên save session trước để lấy BIGINT Id.
             */
            await _db.SaveChangesAsync(
                cancellationToken);

            for (var i = 0;
                 i < vocabularyIds.Count;
                 i++)
            {
                _db.Add(
                    new FlashcardSessionItem(
                        session.Id,
                        vocabularyIds[i],
                        i));
            }

            await _db.SaveChangesAsync(
                cancellationToken);

            await tx.CommitAsync(
                cancellationToken);

            return await GetSessionAsync(
                userId,
                session.PublicId,
                cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(
                cancellationToken);

            throw;
        }
    }

    public async Task<Result<FlashcardSessionResponse>>
        GetSessionAsync(
            Guid userId,
            Guid sessionPublicId,
            CancellationToken cancellationToken = default)
    {
        var session =
            await _db.Set<FlashcardSession>()
                .AsNoTracking()
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId ==
                            sessionPublicId &&
                        x.UserId == userId,
                    cancellationToken);

        if (session is null)
        {
            return Result.Failure<
                FlashcardSessionResponse>(
                Error.NotFound(
                    "Flashcard.SessionNotFound",
                    "Không tìm thấy Flashcard Session."));
        }

        var vocabularyIds =
            session.Items
                .Select(x => x.VocabularyId)
                .Distinct()
                .ToArray();

        var vocabulary =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Where(
                    x => vocabularyIds.Contains(x.Id))
                .ToDictionaryAsync(
                    x => x.Id,
                    cancellationToken);

        var items =
            session.Items
                .OrderBy(x => x.SortOrder)
                .Select(
                    item =>
                    {
                        var word =
                            vocabulary[
                                item.VocabularyId];

                        return new FlashcardItemResponse(
                            item.PublicId,
                            word.PublicId,
                            item.SortOrder,
                            word.Simplified,
                            word.Traditional,
                            word.Pinyin,
                            word.PrimaryMeaningVi,
                            session.Mode,
                            item.IsAnswered,
                            item.Rating,
                            item.WasCorrect,
                            item.ResponseTimeMs);
                    })
                .ToArray();

        return Result.Success(
            new FlashcardSessionResponse(
                session.PublicId,
                session.Mode,
                session.SourceType,
                session.Status,
                session.CurrentIndex,
                session.TotalItems,
                session.CorrectItems,
                session.WrongItems,
                session.AccuracyPercent,
                session.StartedAt,
                session.CompletedAt,
                items));
    }

    public async Task<Result<FlashcardAnswerResponse>>
        AnswerAsync(
            Guid userId,
            Guid sessionPublicId,
            Guid itemPublicId,
            AnswerFlashcardRequest request,
            CancellationToken cancellationToken = default)
    {
        var session =
            await _db.Set<FlashcardSession>()
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId ==
                            sessionPublicId &&
                        x.UserId == userId,
                    cancellationToken);

        if (session is null)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                Error.NotFound(
                    "Flashcard.SessionNotFound",
                    "Không tìm thấy Flashcard Session."));
        }

        if (session.Status !=
            FlashcardSessionStatus.Active)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                Error.Conflict(
                    "Flashcard.SessionNotActive",
                    "Flashcard session không còn Active."));
        }

        var item =
            await _db.Set<FlashcardSessionItem>()
                .FirstOrDefaultAsync(
                    x =>
                        x.PublicId == itemPublicId &&
                        x.FlashcardSessionId ==
                            session.Id,
                    cancellationToken);

        if (item is null)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                Error.NotFound(
                    "Flashcard.ItemNotFound",
                    "Không tìm thấy Flashcard Item."));
        }

        if (item.IsAnswered)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                Error.Conflict(
                    "Flashcard.AlreadyAnswered",
                    "Flashcard Item đã được trả lời."));
        }

        /*
         * Enforce answer đúng thứ tự.
         *
         * CurrentIndex bắt đầu 0.
         */
        if (item.SortOrder !=
            session.CurrentIndex)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                Error.Conflict(
                    "Flashcard.InvalidOrder",
                    "Flashcard phải được trả lời đúng thứ tự."));
        }

        var state =
            await _db.Set<UserVocabularyState>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.VocabularyId ==
                            item.VocabularyId,
                    cancellationToken);

        if (state is null)
        {
            state =
                new UserVocabularyState(
                    userId,
                    item.VocabularyId);

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

        await using var tx =
            await _db.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            item.Answer(
                request.Rating,
                request.WasCorrect,
                request.ResponseTimeMs);

            state.ApplyReview(
                request.Rating,
                request.WasCorrect,
                schedule.MasteryAfter,
                schedule.IntervalAfterMinutes,
                schedule.ReviewedAt,
                schedule.NextReviewAt,
                DateOnly.FromDateTime(
                    reviewedAt.UtcDateTime));

            var reviewEvent =
                new ReviewEvent(
                    userId,
                    item.VocabularyId,
                    request.Rating,
                    request.WasCorrect,
                    schedule.MasteryBefore,
                    schedule.MasteryAfter,
                    schedule.IntervalAfterMinutes,
                    schedule.IntervalBeforeMinutes,
                    request.ResponseTimeMs,
                    session.Id);

            _db.Add(reviewEvent);

            session.RegisterAnswer(
                request.WasCorrect);

            await _db.SaveChangesAsync(
                cancellationToken);

            await tx.CommitAsync(
                cancellationToken);
                
            if (session.Status == FlashcardSessionStatus.Completed)
            {
                await _gamification.AwardXpAsync(
                    userId,
                    GamificationConstants.ReviewCompletedXp,
                    "Hoàn thành ôn tập từ vựng",
                    XpSources.Review,
                    session.PublicId.ToString(),
                    cancellationToken);

                await _gamification.RegisterLearningActivityAsync(
                    userId,
                    DateTimeOffset.UtcNow,
                    cancellationToken);

                await _achievementEvaluator.EvaluateAsync(
                    userId,
                    cancellationToken);
            }
        }
        catch
        {
            await tx.RollbackAsync(
                cancellationToken);

            throw;
        }

        var loadedSession =
            await GetSessionAsync(
                userId,
                sessionPublicId,
                cancellationToken);

        if (loadedSession.IsFailure)
        {
            return Result.Failure<
                FlashcardAnswerResponse>(
                loadedSession.Error);
        }

        var vocabularyPublicId =
            await _db.Set<
                    Domain.Entities.Vocabulary.Vocabulary>()
                .AsNoTracking()
                .Where(
                    x =>
                        x.Id ==
                        item.VocabularyId)
                .Select(x => x.PublicId)
                .FirstAsync(
                    cancellationToken);

        var reviewResponse =
            new ReviewResultResponse(
                vocabularyPublicId,
                request.Rating,
                request.WasCorrect,
                schedule.MasteryBefore,
                schedule.MasteryAfter,
                schedule.IntervalBeforeMinutes,
                schedule.IntervalAfterMinutes,
                schedule.NextReviewAt,
                state.LearningState);

        return Result.Success(
            new FlashcardAnswerResponse(
                loadedSession.Value,
                reviewResponse));
    }

    public async Task<Result> AbandonAsync(
        Guid userId,
        Guid sessionPublicId,
        CancellationToken cancellationToken = default)
    {
        var session =
            await _db.Set<FlashcardSession>()
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId == userId &&
                        x.PublicId ==
                            sessionPublicId,
                    cancellationToken);

        if (session is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Flashcard.SessionNotFound",
                    "Không tìm thấy Flashcard Session."));
        }

        session.Abandon();

        await _db.SaveChangesAsync(
            cancellationToken);

        return Result.Success();
    }

    private async Task<Result<List<long>>>
        ResolveVocabularyIdsAsync(
            Guid userId,
            CreateFlashcardSessionRequest request,
            int limit,
            CancellationToken cancellationToken)
    {
        switch (request.SourceType)
        {
            case FlashcardSourceType.ReviewQueue:
            {
                var now =
                    DateTimeOffset.UtcNow;

                var ids =
                    await _db.Set<UserVocabularyState>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.UserId == userId &&
                                (
                                    x.NextReviewAt == null ||
                                    x.NextReviewAt <= now
                                ) &&
                                x.Vocabulary.Status ==
                                    ContentStatus.Published)
                        .OrderBy(
                            x =>
                                x.NextReviewAt ??
                                DateTimeOffset.MinValue)
                        .Take(limit)
                        .Select(
                            x => x.VocabularyId)
                        .ToListAsync(
                            cancellationToken);

                return Result.Success(ids);
            }

            case FlashcardSourceType.Lesson:
            {
                if (!request.SourcePublicId.HasValue)
                {
                    return Result.Failure<List<long>>(
                        Error.Validation(
                            "Flashcard.SourceRequired",
                            "Lesson PublicId là bắt buộc."));
                }

                var lessonId =
                    await _db.Set<HanYu.Domain.Entities.Lesson.Lesson>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.PublicId ==
                                    request.SourcePublicId.Value &&
                                x.Status ==
                                    ContentStatus.Published)
                        .Select(x => (long?)x.Id)
                        .FirstOrDefaultAsync(
                            cancellationToken);

                if (!lessonId.HasValue)
                {
                    return Result.Failure<List<long>>(
                        Error.NotFound(
                            "Lesson.NotFound",
                            "Không tìm thấy Lesson."));
                }

                var ids =
                    await _db.Set<LessonVocabulary>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.LessonId ==
                                    lessonId.Value &&
                                x.Vocabulary.Status ==
                                    ContentStatus.Published)
                        .OrderBy(x => x.SortOrder)
                        .Take(limit)
                        .Select(x => x.VocabularyId)
                        .ToListAsync(
                            cancellationToken);

                return Result.Success(ids);
            }

            case FlashcardSourceType.Topic:
            {
                if (!request.SourcePublicId.HasValue)
                {
                    return Result.Failure<List<long>>(
                        Error.Validation(
                            "Flashcard.SourceRequired",
                            "Topic PublicId là bắt buộc."));
                }

                var topicId =
                    await _db.Set<Topic>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.PublicId ==
                                request.SourcePublicId.Value &&
                                x.Status ==
                                    ContentStatus.Published)
                        .Select(x => (long?)x.Id)
                        .FirstOrDefaultAsync(
                            cancellationToken);

                if (!topicId.HasValue)
                {
                    return Result.Failure<List<long>>(
                        Error.NotFound(
                            "Topic.NotFound",
                            "Không tìm thấy Topic."));
                }

                var ids =
                    await _db.Set<
                            Domain.Entities.Vocabulary.Vocabulary>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.TopicId ==
                                    topicId.Value &&
                                x.Status ==
                                    ContentStatus.Published)
                        .OrderBy(
                            x => x.Simplified)
                        .Take(limit)
                        .Select(x => x.Id)
                        .ToListAsync(
                            cancellationToken);

                return Result.Success(ids);
            }

            case FlashcardSourceType.Custom:
            {
                if (request.VocabularyPublicIds is null ||
                    request.VocabularyPublicIds.Count == 0)
                {
                    return Result.Failure<List<long>>(
                        Error.Validation(
                            "Flashcard.VocabularyRequired",
                            "Custom session phải có vocabulary."));
                }

                var publicIds =
                    request.VocabularyPublicIds
                        .Distinct()
                        .Take(limit)
                        .ToArray();

                var values =
                    await _db.Set<
                            Domain.Entities.Vocabulary.Vocabulary>()
                        .AsNoTracking()
                        .Where(
                            x =>
                                publicIds.Contains(
                                    x.PublicId) &&
                                x.Status ==
                                    ContentStatus.Published)
                        .Select(
                            x => new
                            {
                                x.Id,
                                x.PublicId
                            })
                        .ToArrayAsync(
                            cancellationToken);

                var map =
                    values.ToDictionary(
                        x => x.PublicId,
                        x => x.Id);

                var result =
                    publicIds
                        .Where(map.ContainsKey)
                        .Select(x => map[x])
                        .ToList();

                return Result.Success(result);
            }

            default:
                return Result.Failure<List<long>>(
                    Error.Validation(
                        "Flashcard.InvalidSource",
                        "Flashcard source không hợp lệ."));
        }
    }
}
