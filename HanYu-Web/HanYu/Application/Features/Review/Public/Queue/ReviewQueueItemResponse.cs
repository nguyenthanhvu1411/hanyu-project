using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Public.Queue;

public sealed record ReviewQueueItemResponse(
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    long HskLevel,
    LearningState LearningState,
    decimal MasteryScore,
    int CorrectCount,
    int WrongCount,
    int ConsecutiveCorrect,
    DateTimeOffset? LastReviewedAt,
    DateTimeOffset? NextReviewAt,
    int? CurrentIntervalMinutes,
    bool IsFavorite);
