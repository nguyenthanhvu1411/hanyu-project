namespace HanYu.Application.Features.Review.Admin.Users;

public sealed record AdminUserReviewSummaryResponse(
    Guid UserId,
    int TotalVocabulary,
    int LearningVocabulary,
    int KnownVocabulary,
    int MasteredVocabulary,
    int DueVocabulary,
    int OverdueVocabulary,
    int FavoriteVocabulary,
    long TotalReviews,
    long CorrectReviews,
    long WrongReviews,
    decimal OverallAccuracy,
    DateTimeOffset? LastReviewedAt,
    int ActiveFlashcardSessions);
