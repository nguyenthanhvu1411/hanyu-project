namespace HanYu.Application.Features.Analytics.Admin.Dashboard;

public sealed record AdminAnalyticsDashboardResponse(
    int ActiveUsersToday,
    long LearningSecondsToday,
    long LessonsCompletedToday,
    long VocabularyReviewedToday,
    long QuizAttemptsToday,
    long QuizPassedToday,
    long AiInteractionsToday,
    long XpEarnedToday);
