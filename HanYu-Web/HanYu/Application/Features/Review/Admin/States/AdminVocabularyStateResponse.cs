using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.States;

public sealed record AdminVocabularyStateResponse(
    Guid UserId,
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PrimaryMeaningVi,
    long HskLevelId,
    LearningState LearningState,
    bool IsFavorite,
    decimal MasteryScore,
    int CorrectCount,
    int WrongCount,
    int ConsecutiveCorrect,
    int DistinctCorrectDays,
    DateTimeOffset? LastCorrectAt,
    DateTimeOffset? LastReviewedAt,
    DateTimeOffset? NextReviewAt,
    int? CurrentIntervalMinutes,
    DateTimeOffset? FirstLearnedAt,
    DateTimeOffset? MasteredAt,
    DateTimeOffset UpdatedAt);
