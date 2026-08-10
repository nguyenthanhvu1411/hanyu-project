using HanYu.Application.Common.Models;
using HanYu.Application.Features.Review.Admin.Dashboard;
using HanYu.Application.Features.Review.Admin.Events;
using HanYu.Application.Features.Review.Admin.Flashcards;
using HanYu.Application.Features.Review.Admin.States;
using HanYu.Application.Features.Review.Admin.Users;
using HanYu.Application.Features.Review.Mapping;
using HanYu.Application.Interfaces.Review;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Review;

public sealed class ReviewAdminService : IReviewAdminService
{
    private readonly HanYuDbContext _db;

    public ReviewAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // DASHBOARD
    // =========================================================

    public async Task<Result<AdminReviewDashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var startOfDay = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset);
        var states = _db.Set<UserVocabularyState>().AsNoTracking();

        var totalStates = await states.LongCountAsync(cancellationToken);
        var dueReviews = await states.LongCountAsync(x => x.NextReviewAt.HasValue && x.NextReviewAt <= now, cancellationToken);
        var overdueReviews = await states.LongCountAsync(x => x.NextReviewAt.HasValue && x.NextReviewAt < now.AddHours(-24), cancellationToken);

        var learning = await states.LongCountAsync(x => x.LearningState == LearningState.Learning, cancellationToken);
        var known = await states.LongCountAsync(x => x.LearningState == LearningState.Known, cancellationToken);
        var mastered = await states.LongCountAsync(x => x.LearningState == LearningState.Mastered, cancellationToken);
        var favorites = await states.LongCountAsync(x => x.IsFavorite, cancellationToken);

        var todayEvents = _db.Set<ReviewEvent>().AsNoTracking().Where(x => x.ReviewedAt >= startOfDay);
        var reviewCount = await todayEvents.LongCountAsync(cancellationToken);
        var correctToday = await todayEvents.LongCountAsync(x => x.WasCorrect, cancellationToken);
        var wrongToday = reviewCount - correctToday;
        var accuracy = reviewCount == 0 ? 0m : Math.Round(correctToday * 100m / reviewCount, 2);

        var sessions = _db.Set<FlashcardSession>().AsNoTracking();
        var activeSessions = await sessions.LongCountAsync(x => x.Status == FlashcardSessionStatus.Active, cancellationToken);
        var completedToday = await sessions.LongCountAsync(x => x.Status == FlashcardSessionStatus.Completed && x.CompletedAt.HasValue && x.CompletedAt.Value >= startOfDay, cancellationToken);
        var abandonedToday = await sessions.LongCountAsync(x => x.Status == FlashcardSessionStatus.Abandoned && x.CompletedAt.HasValue && x.CompletedAt.Value >= startOfDay, cancellationToken);

        return Result.Success(new AdminReviewDashboardResponse(
            totalStates, dueReviews, overdueReviews, learning, known, mastered, favorites,
            reviewCount, correctToday, wrongToday, accuracy, activeSessions, completedToday, abandonedToday));
    }

    // =========================================================
    // STATES
    // =========================================================

    public async Task<Result<PagedResult<AdminVocabularyStateResponse>>> GetStatesAsync(AdminVocabularyStateQuery query, CancellationToken cancellationToken = default)
    {
        var source = _db.Set<UserVocabularyState>().AsNoTracking().Include(x => x.Vocabulary).AsQueryable();

        if (query.UserId.HasValue) source = source.Where(x => x.UserId == query.UserId.Value);
        if (query.VocabularyId.HasValue) source = source.Where(x => x.VocabularyId == query.VocabularyId.Value);

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var q = query.Q.Trim();
            source = source.Where(x =>
                EF.Functions.ILike(x.Vocabulary.Simplified, $"%{q}%") ||
                (x.Vocabulary.Traditional != null && EF.Functions.ILike(x.Vocabulary.Traditional, $"%{q}%")) ||
                EF.Functions.ILike(x.Vocabulary.Pinyin, $"%{q}%") ||
                EF.Functions.ILike(x.Vocabulary.PinyinNormalized, $"%{q}%") ||
                EF.Functions.ILike(x.Vocabulary.PrimaryMeaningVi, $"%{q}%"));
        }

        if (query.HskLevelId.HasValue) source = source.Where(x => x.Vocabulary.HskLevelId == query.HskLevelId.Value);
        if (query.TopicId.HasValue) source = source.Where(x => x.Vocabulary.TopicId == query.TopicId.Value);
        if (query.LearningState.HasValue) source = source.Where(x => x.LearningState == query.LearningState.Value);
        if (query.IsFavorite.HasValue) source = source.Where(x => x.IsFavorite == query.IsFavorite.Value);

        var now = DateTimeOffset.UtcNow;
        if (query.IsDue == true) source = source.Where(x => x.NextReviewAt.HasValue && x.NextReviewAt <= now);
        if (query.IsDue == false) source = source.Where(x => !x.NextReviewAt.HasValue || x.NextReviewAt > now);
        if (query.IsOverdue == true)
        {
            var overdueBefore = now.AddHours(-24);
            source = source.Where(x => x.NextReviewAt.HasValue && x.NextReviewAt < overdueBefore);
        }

        if (query.MinMastery.HasValue) source = source.Where(x => x.MasteryScore >= query.MinMastery.Value);
        if (query.MaxMastery.HasValue) source = source.Where(x => x.MasteryScore <= query.MaxMastery.Value);

        source = ApplyStateSort(source, query.Sort);
        var total = await source.LongCountAsync(cancellationToken);
        var values = await source.Skip((query.NormalizedPage - 1) * query.NormalizedPageSize).Take(query.NormalizedPageSize).ToArrayAsync(cancellationToken);

        var result = new PagedResult<AdminVocabularyStateResponse>(
            values.Select(ReviewAdminMapper.ToStateResponse).ToArray(),
            query.NormalizedPage, query.NormalizedPageSize, total);

        return Result.Success(result);
    }

    public async Task<Result<AdminVocabularyStateDetailResponse>> GetStateAsync(Guid userId, long vocabularyId, CancellationToken cancellationToken = default)
    {
        var state = await _db.Set<UserVocabularyState>().AsNoTracking().Include(x => x.Vocabulary)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.VocabularyId == vocabularyId, cancellationToken);

        if (state is null) return Result.Failure<AdminVocabularyStateDetailResponse>(Error.NotFound("Review.StateNotFound", "Không tìm thấy trạng thái SRS."));
        return Result.Success(ReviewAdminMapper.ToStateDetail(state));
    }

    public async Task<Result> ResetStateAsync(Guid userId, long vocabularyId, AdminResetVocabularyStateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure(Error.Validation("Review.ResetReasonRequired", "Phải nhập lý do reset progress."));

        var state = await _db.Set<UserVocabularyState>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.VocabularyId == vocabularyId, cancellationToken);

        if (state is null) return Result.Failure(Error.NotFound("Review.StateNotFound", "Không tìm thấy trạng thái SRS."));

        state.ResetProgress();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // =========================================================
    // REVIEW EVENTS
    // =========================================================

    public async Task<Result<PagedResult<AdminReviewEventResponse>>> GetEventsAsync(AdminReviewEventQuery query, CancellationToken cancellationToken = default)
    {
        var source = _db.Set<ReviewEvent>().AsNoTracking().AsQueryable();

        if (query.UserId.HasValue) source = source.Where(x => x.UserId == query.UserId.Value);
        if (query.VocabularyId.HasValue) source = source.Where(x => x.VocabularyId == query.VocabularyId.Value);
        if (query.FlashcardSessionId.HasValue) source = source.Where(x => x.FlashcardSessionId == query.FlashcardSessionId.Value);
        if (query.Rating.HasValue) source = source.Where(x => x.Rating == query.Rating.Value);
        if (query.WasCorrect.HasValue) source = source.Where(x => x.WasCorrect == query.WasCorrect.Value);
        if (query.From.HasValue) source = source.Where(x => x.ReviewedAt >= query.From.Value);
        if (query.To.HasValue) source = source.Where(x => x.ReviewedAt <= query.To.Value);
        if (query.MinMasteryAfter.HasValue) source = source.Where(x => x.MasteryAfter >= query.MinMasteryAfter.Value);
        if (query.MaxMasteryAfter.HasValue) source = source.Where(x => x.MasteryAfter <= query.MaxMasteryAfter.Value);

        source = ApplyEventSort(source, query.Sort);
        var total = await source.LongCountAsync(cancellationToken);
        var events = await source.Skip((query.NormalizedPage - 1) * query.NormalizedPageSize).Take(query.NormalizedPageSize).ToArrayAsync(cancellationToken);

        var vocabularyIds = events.Select(x => x.VocabularyId).Distinct().ToArray();
        var vocabularies = await _db.Set<Domain.Entities.Vocabulary.Vocabulary>().AsNoTracking()
            .Where(x => vocabularyIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

        var sessionIds = events.Where(x => x.FlashcardSessionId.HasValue).Select(x => x.FlashcardSessionId!.Value).Distinct().ToArray();
        var sessions = await _db.Set<FlashcardSession>().AsNoTracking()
            .Where(x => sessionIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

        var response = events.Select(x =>
        {
            var vocabulary = vocabularies[x.VocabularyId];
            Guid? sessionPublicId = null;
            if (x.FlashcardSessionId.HasValue && sessions.TryGetValue(x.FlashcardSessionId.Value, out var session))
                sessionPublicId = session.PublicId;

            return new AdminReviewEventResponse(
                x.Id, x.PublicId, x.UserId, x.VocabularyId, vocabulary.PublicId,
                vocabulary.Simplified, vocabulary.Pinyin, vocabulary.PrimaryMeaningVi,
                x.FlashcardSessionId, sessionPublicId, x.Rating, x.WasCorrect,
                x.ResponseTimeMs, x.MasteryBefore, x.MasteryAfter, x.IntervalBeforeMinutes, x.IntervalAfterMinutes, x.ReviewedAt);
        }).ToArray();

        return Result.Success(new PagedResult<AdminReviewEventResponse>(response, query.NormalizedPage, query.NormalizedPageSize, total));
    }

    public async Task<Result<AdminReviewEventResponse>> GetEventAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ReviewEvent>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure<AdminReviewEventResponse>(Error.NotFound("Review.EventNotFound", "Không tìm thấy ReviewEvent."));

        var vocabulary = await _db.Set<Domain.Entities.Vocabulary.Vocabulary>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.VocabularyId, cancellationToken);
        if (vocabulary is null) return Result.Failure<AdminReviewEventResponse>(Error.NotFound("Vocabulary.NotFound", "Vocabulary của ReviewEvent không tồn tại."));

        Guid? sessionPublicId = null;
        if (entity.FlashcardSessionId.HasValue)
        {
            sessionPublicId = await _db.Set<FlashcardSession>().AsNoTracking()
                .Where(x => x.Id == entity.FlashcardSessionId.Value)
                .Select(x => (Guid?)x.PublicId).FirstOrDefaultAsync(cancellationToken);
        }

        return Result.Success(new AdminReviewEventResponse(
            entity.Id, entity.PublicId, entity.UserId, entity.VocabularyId, vocabulary.PublicId,
            vocabulary.Simplified, vocabulary.Pinyin, vocabulary.PrimaryMeaningVi,
            entity.FlashcardSessionId, sessionPublicId, entity.Rating, entity.WasCorrect,
            entity.ResponseTimeMs, entity.MasteryBefore, entity.MasteryAfter, entity.IntervalBeforeMinutes, entity.IntervalAfterMinutes, entity.ReviewedAt));
    }

    // =========================================================
    // FLASHCARD SESSIONS
    // =========================================================

    public async Task<Result<PagedResult<AdminFlashcardSessionResponse>>> GetFlashcardSessionsAsync(AdminFlashcardSessionQuery query, CancellationToken cancellationToken = default)
    {
        var source = _db.Set<FlashcardSession>().AsNoTracking().AsQueryable();

        if (query.UserId.HasValue) source = source.Where(x => x.UserId == query.UserId.Value);
        if (query.Mode.HasValue) source = source.Where(x => x.Mode == query.Mode.Value);
        if (query.SourceType.HasValue) source = source.Where(x => x.SourceType == query.SourceType.Value);
        if (query.Status.HasValue) source = source.Where(x => x.Status == query.Status.Value);
        if (query.SourceId.HasValue) source = source.Where(x => x.SourceId == query.SourceId.Value);
        if (query.From.HasValue) source = source.Where(x => x.StartedAt >= query.From.Value);
        if (query.To.HasValue) source = source.Where(x => x.StartedAt <= query.To.Value);

        source = ApplySessionSort(source, query.Sort);
        var total = await source.LongCountAsync(cancellationToken);
        var sessions = await source.Skip((query.NormalizedPage - 1) * query.NormalizedPageSize).Take(query.NormalizedPageSize).ToArrayAsync(cancellationToken);

        var items = sessions.Select(x => new AdminFlashcardSessionResponse(
            x.Id, x.PublicId, x.UserId, x.Mode, x.SourceType, x.SourceId, x.Status,
            x.CurrentIndex, x.TotalItems, x.CorrectItems, x.WrongItems, x.AccuracyPercent,
            x.StartedAt, x.CompletedAt, x.CreatedAt, x.UpdatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminFlashcardSessionResponse>(items, query.NormalizedPage, query.NormalizedPageSize, total));
    }

    public async Task<Result<AdminFlashcardSessionDetailResponse>> GetFlashcardSessionAsync(long id, CancellationToken cancellationToken = default)
    {
        var session = await _db.Set<FlashcardSession>().AsNoTracking().Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (session is null) return Result.Failure<AdminFlashcardSessionDetailResponse>(Error.NotFound("Flashcard.SessionNotFound", "Không tìm thấy Flashcard Session."));

        var vocabularyIds = session.Items.Select(x => x.VocabularyId).Distinct().ToArray();
        var vocabulary = await _db.Set<Domain.Entities.Vocabulary.Vocabulary>().AsNoTracking()
            .Where(x => vocabularyIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);

        var items = session.Items.OrderBy(x => x.SortOrder).Select(x =>
        {
            var word = vocabulary[x.VocabularyId];
            return new AdminFlashcardSessionItemResponse(
                x.Id, x.PublicId, x.VocabularyId, word.PublicId,
                word.Simplified, word.Traditional, word.Pinyin, word.PrimaryMeaningVi,
                x.SortOrder, x.IsAnswered, x.Rating, x.WasCorrect, x.ResponseTimeMs, x.AnsweredAt);
        }).ToArray();

        return Result.Success(new AdminFlashcardSessionDetailResponse(
            session.Id, session.PublicId, session.UserId, session.Mode, session.SourceType,
            session.SourceId, session.Status, session.CurrentIndex, session.TotalItems,
            session.CorrectItems, session.WrongItems, session.AccuracyPercent,
            session.StartedAt, session.CompletedAt, items));
    }

    public async Task<Result> AbandonFlashcardSessionAsync(long id, CancellationToken cancellationToken = default)
    {
        var session = await _db.Set<FlashcardSession>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (session is null) return Result.Failure(Error.NotFound("Flashcard.SessionNotFound", "Không tìm thấy Flashcard Session."));
        if (session.Status != FlashcardSessionStatus.Active) return Result.Failure(Error.Conflict("Flashcard.SessionNotActive", "Chỉ session Active mới có thể Abandon."));

        session.Abandon();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    // =========================================================
    // USER SUMMARY
    // =========================================================

    public async Task<Result<AdminUserReviewSummaryResponse>> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty) return Result.Failure<AdminUserReviewSummaryResponse>(Error.Validation("Review.InvalidUserId", "UserId không hợp lệ."));

        var now = DateTimeOffset.UtcNow;
        var states = _db.Set<UserVocabularyState>().AsNoTracking().Where(x => x.UserId == userId);

        var total = await states.CountAsync(cancellationToken);
        var learning = await states.CountAsync(x => x.LearningState == LearningState.Learning, cancellationToken);
        var known = await states.CountAsync(x => x.LearningState == LearningState.Known, cancellationToken);
        var mastered = await states.CountAsync(x => x.LearningState == LearningState.Mastered, cancellationToken);
        var due = await states.CountAsync(x => x.NextReviewAt.HasValue && x.NextReviewAt <= now, cancellationToken);
        var overdue = await states.CountAsync(x => x.NextReviewAt.HasValue && x.NextReviewAt < now.AddHours(-24), cancellationToken);
        var favorite = await states.CountAsync(x => x.IsFavorite, cancellationToken);

        var events = _db.Set<ReviewEvent>().AsNoTracking().Where(x => x.UserId == userId);
        var reviews = await events.LongCountAsync(cancellationToken);
        var correct = await events.LongCountAsync(x => x.WasCorrect, cancellationToken);
        var wrong = reviews - correct;
        var accuracy = reviews == 0 ? 0m : Math.Round(correct * 100m / reviews, 2);
        var lastReviewed = await events.OrderByDescending(x => x.ReviewedAt).Select(x => (DateTimeOffset?)x.ReviewedAt).FirstOrDefaultAsync(cancellationToken);

        var activeSessions = await _db.Set<FlashcardSession>().AsNoTracking()
            .CountAsync(x => x.UserId == userId && x.Status == FlashcardSessionStatus.Active, cancellationToken);

        return Result.Success(new AdminUserReviewSummaryResponse(
            userId, total, learning, known, mastered, due, overdue, favorite,
            reviews, correct, wrong, accuracy, lastReviewed, activeSessions));
    }

    // =========================================================
    // SORT
    // =========================================================

    private static IQueryable<UserVocabularyState> ApplyStateSort(IQueryable<UserVocabularyState> query, string? sort)
    {
        return sort switch
        {
            "nextReviewAt" => query.OrderBy(x => x.NextReviewAt),
            "-nextReviewAt" => query.OrderByDescending(x => x.NextReviewAt),
            "mastery" => query.OrderBy(x => x.MasteryScore),
            "-mastery" => query.OrderByDescending(x => x.MasteryScore),
            "lastReviewedAt" => query.OrderBy(x => x.LastReviewedAt),
            "-lastReviewedAt" => query.OrderByDescending(x => x.LastReviewedAt),
            _ => query.OrderBy(x => x.NextReviewAt)
        };
    }

    private static IQueryable<ReviewEvent> ApplyEventSort(IQueryable<ReviewEvent> query, string? sort)
    {
        return sort switch
        {
            "reviewedAt" => query.OrderBy(x => x.ReviewedAt),
            "-reviewedAt" => query.OrderByDescending(x => x.ReviewedAt),
            "masteryAfter" => query.OrderBy(x => x.MasteryAfter),
            "-masteryAfter" => query.OrderByDescending(x => x.MasteryAfter),
            _ => query.OrderByDescending(x => x.ReviewedAt)
        };
    }

    private static IQueryable<FlashcardSession> ApplySessionSort(IQueryable<FlashcardSession> query, string? sort)
    {
        return sort switch
        {
            "startedAt" => query.OrderBy(x => x.StartedAt),
            "-startedAt" => query.OrderByDescending(x => x.StartedAt),
            "completedAt" => query.OrderBy(x => x.CompletedAt),
            "-completedAt" => query.OrderByDescending(x => x.CompletedAt),
            "accuracy" => query.OrderBy(x => x.AccuracyPercent),
            "-accuracy" => query.OrderByDescending(x => x.AccuracyPercent),
            _ => query.OrderByDescending(x => x.StartedAt)
        };
    }
}
