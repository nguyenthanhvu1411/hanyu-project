using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Review.Admin.Dashboard;

public sealed record AdminReviewDashboardResponse(
    long TotalVocabularyStates,
    long DueReviews,
    long OverdueReviews,
    long LearningVocabulary,
    long KnownVocabulary,
    long MasteredVocabulary,
    long FavoriteVocabulary,
    long ReviewsToday,
    long CorrectReviewsToday,
    long WrongReviewsToday,
    decimal AccuracyToday,
    long ActiveFlashcardSessions,
    long CompletedFlashcardSessionsToday,
    long AbandonedFlashcardSessionsToday);
