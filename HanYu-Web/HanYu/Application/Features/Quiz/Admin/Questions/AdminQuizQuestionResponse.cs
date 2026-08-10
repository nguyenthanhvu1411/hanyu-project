using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Questions;

public sealed record AdminQuizQuestionResponse(
    long Id,
    Guid PublicId,
    long QuizId,
    long? VocabularyId,
    Guid? VocabularyPublicId,
    QuizQuestionType QuestionType,
    string Prompt,
    string? PromptPinyin,
    string? CorrectAnswerText,
    string? ExplanationVi,
    string? HintVi,
    decimal Points,
    int SortOrder,
    int? TimeLimitSeconds,
    bool IsRequired,
    ContentStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
