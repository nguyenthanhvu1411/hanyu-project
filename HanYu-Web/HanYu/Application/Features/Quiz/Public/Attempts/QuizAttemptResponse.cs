using HanYu.Domain.Enums;
using HanYu.Application.Features.Quiz.Public.Questions;

namespace HanYu.Application.Features.Quiz.Public.Attempts;

public sealed record StartQuizAttemptRequest(
    string IdempotencyKey);

public sealed record QuizAttemptResponse(
    Guid PublicId,
    Guid QuizPublicId,
    int AttemptNumber,
    QuizAttemptStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? ExpiresAt,
    int CurrentQuestion,
    int TotalQuestions,
    IReadOnlyCollection<QuizAttemptQuestionResponse> Questions);
