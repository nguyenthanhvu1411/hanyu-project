using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Review;

public sealed record VocabularyLearningStateResponse(
    Guid VocabularyPublicId,
    LearningState LearningState,
    bool IsFavorite,
    decimal MasteryScore,
    int CorrectCount,
    int WrongCount,
    int ConsecutiveCorrect,
    int DistinctCorrectDays,
    DateTimeOffset? FirstLearnedAt,
    DateTimeOffset? LastReviewedAt,
    DateTimeOffset? NextReviewAt,
    int? CurrentIntervalMinutes,
    DateTimeOffset? MasteredAt);
