using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Attempts;

public sealed record AdminQuizAttemptQuery : PaginationRequest
{
    public Guid? UserId { get; init; }
    public long? QuizId { get; init; }
    public QuizAttemptStatus? Status { get; init; }
    public bool? IsPassed { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
}

public sealed record AdminQuizAttemptResponse(
    long Id,
    Guid UserId,
    string UserDisplayName,
    string UserEmail,
    long QuizId,
    string QuizTitleVi,
    int AttemptNumber,
    QuizAttemptStatus Status,
    decimal? Score,
    decimal? MaxScore,
    decimal? Percentage,
    bool? IsPassed,
    int CorrectAnswers,
    int WrongAnswers,
    int UnansweredQuestions,
    DateTimeOffset StartedAt,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ExpiresAt,
    int? DurationSeconds);

public sealed record AdminQuizAttemptAnswerResponse(
    long Id,
    string QuestionPrompt,
    string? QuestionPinyin,
    string? AnswerText,
    bool? IsCorrect,
    decimal? EarnedPoints,
    int? ResponseTimeMs,
    DateTimeOffset? AnsweredAt);

public sealed record AdminQuizAttemptDetailResponse(
    AdminQuizAttemptResponse Attempt,
    IReadOnlyCollection<AdminQuizAttemptAnswerResponse> Answers);

public sealed record AdminQuizAttemptStatisticsResponse(
    long TotalAttempts,
    long InProgressAttempts,
    long SubmittedAttempts,
    long PassedAttempts,
    long FailedAttempts,
    decimal AveragePercentage,
    decimal PassRatePercent,
    long AttemptsToday);