using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Questions;

public sealed record UpdateQuizQuestionRequest(
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
    long? VocabularyId);
