using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Events;

public sealed record AdminReviewEventResponse(
    long Id,
    Guid PublicId,
    Guid UserId,
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,
    string Pinyin,
    string PrimaryMeaningVi,
    long? FlashcardSessionId,
    Guid? FlashcardSessionPublicId,
    ReviewRating Rating,
    bool WasCorrect,
    int? ResponseTimeMs,
    decimal MasteryBefore,
    decimal MasteryAfter,
    int? IntervalBeforeMinutes,
    int IntervalAfterMinutes,
    DateTimeOffset ReviewedAt);
