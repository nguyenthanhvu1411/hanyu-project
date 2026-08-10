namespace HanYu.Application.Features.Quiz.Public.Results;

public sealed record QuizResultResponse(
    Guid AttemptPublicId,
    Guid QuizPublicId,
    decimal Score,
    decimal MaxScore,
    decimal Percentage,
    bool IsPassed,
    int CorrectAnswers,
    int WrongAnswers,
    int UnansweredQuestions,
    int DurationSeconds,
    DateTimeOffset SubmittedAt,
    IReadOnlyCollection<QuizQuestionResultResponse> Questions);

public sealed record QuizQuestionResultResponse(
    Guid QuestionPublicId,
    string Prompt,
    bool? IsCorrect,
    decimal EarnedPoints,
    decimal MaxPoints,
    string? CorrectAnswer,
    string? ExplanationVi);
