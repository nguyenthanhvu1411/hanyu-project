using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.States;

public sealed record AdminVocabularyStateDetailResponse(
    Guid UserId,
    long VocabularyId,
    Guid VocabularyPublicId,
    string Simplified,
    string? Traditional,
    string Pinyin,
    string PinyinNormalized,
    string PrimaryMeaningVi,
    long HskLevelId,
    LearningState LearningState,
    bool IsFavorite,
    decimal MasteryScore,
    int CorrectCount,
    int WrongCount,
    int TotalReviews,
    int ConsecutiveCorrect,
    int DistinctCorrectDays,
    decimal AccuracyPercent,
    DateTimeOffset? FirstLearnedAt,
    DateTimeOffset? LastCorrectAt,
    DateTimeOffset? LastReviewedAt,
    DateTimeOffset? NextReviewAt,
    DateTimeOffset? MasteredAt,
    int? CurrentIntervalMinutes,
    bool IsDue,
    bool IsOverdue,
    DateTimeOffset UpdatedAt);
