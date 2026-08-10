namespace HanYu.Application.Features.Analytics.Public.Me;

public sealed record MyLearningStatResponse(
    DateOnly Date,
    int LearningSeconds,
    int LessonsStarted,
    int LessonsCompleted,
    int VocabularyReviewed,
    int VocabularyLearned,
    int CorrectReviews,
    int WrongReviews,
    int QuizAttempts,
    int QuizPassed,
    int AiInteractions,
    int XpEarned);

public sealed record MyLearningSummaryResponse(
    int TotalLearningSeconds,
    int LessonsCompleted,
    int VocabularyReviewed,
    int VocabularyLearned,
    int QuizAttempts,
    int QuizPassed,
    int AiInteractions,
    int XpEarned,
    decimal ReviewAccuracy,
    int CurrentStreak,
    int LongestStreak,
    int TotalActiveDays,
    DateOnly? LastLearningDate);
