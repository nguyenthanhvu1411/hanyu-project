namespace HanYu.Application.Features.Learning.Public.Summary;

public sealed record LearningSummaryResponse(
    int TotalLearningMinutes,
    int TotalLessonsCompleted,
    int TotalVocabularyLearned,
    int TotalVocabularyMastered,
    int TotalReviews,
    int TotalQuizAttempts,
    int TotalQuizPassed,
    int TotalXp,
    short CurrentHskLevel,
    decimal OverallMasteryPercent,
    DateTimeOffset? LastLearningAt);
