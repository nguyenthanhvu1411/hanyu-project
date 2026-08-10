using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Review;

public sealed record ReviewResultResponse(
    Guid VocabularyPublicId,
    ReviewRating Rating,
    bool WasCorrect,
    decimal MasteryBefore,
    decimal MasteryAfter,
    int? IntervalBeforeMinutes,
    int IntervalAfterMinutes,
    DateTimeOffset NextReviewAt,
    LearningState LearningState);
