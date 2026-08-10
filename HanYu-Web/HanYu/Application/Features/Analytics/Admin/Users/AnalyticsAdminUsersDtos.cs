using HanYu.Application.Common.Models;

namespace HanYu.Application.Features.Analytics.Admin.Users;

public sealed record AdminLearningStatQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    public string? Sort { get; init; } = "-date";
}

public sealed record AdminDailyLearningStatResponse(
    Guid UserId,
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
    int XpEarned,
    DateTimeOffset UpdatedAt);
