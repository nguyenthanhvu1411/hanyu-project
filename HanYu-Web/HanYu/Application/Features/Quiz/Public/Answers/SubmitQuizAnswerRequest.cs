namespace HanYu.Application.Features.Quiz.Public.Answers;

public sealed record SubmitQuizAnswerRequest(
    Guid? SelectedOptionPublicId,
    string? AnswerText,
    string? AnswerJson,
    int? ResponseTimeMs);

public sealed record QuizAnswerResultResponse(
    bool Accepted,
    bool? IsCorrect,
    decimal? EarnedPoints,
    string? CorrectAnswer,
    string? ExplanationVi);
