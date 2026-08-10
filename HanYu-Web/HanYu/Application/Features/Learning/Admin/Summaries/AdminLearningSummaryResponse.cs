namespace HanYu.Application.Features.Learning.Admin.Summaries;

public sealed record AdminLearningSummaryResponse(
    Guid UserId,
    int TotalLearningSeconds,
    int TotalLessonsCompleted,
    int TotalVocabularyLearned,
    int TotalVocabularyMastered,
    int TotalReviews,
    int TotalQuizAttempts,
    int TotalQuizPassed,
    int TotalXp,
    short CurrentHskLevel,
    decimal OverallMasteryPercent,
    DateTimeOffset? LastLearningAt,
    DateTimeOffset UpdatedAt);
