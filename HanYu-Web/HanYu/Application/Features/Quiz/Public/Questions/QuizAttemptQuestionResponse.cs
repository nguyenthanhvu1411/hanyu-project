using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Public.Questions;

public sealed record QuizAttemptQuestionResponse(
    Guid PublicId,
    Guid QuestionPublicId,
    QuizQuestionType QuestionType,
    string Prompt,
    string? PromptPinyin,
    string? HintVi,
    decimal Points,
    int SortOrder,
    int? TimeLimitSeconds,
    IReadOnlyCollection<QuizAttemptOptionResponse> Options,
    IReadOnlyCollection<QuizAttemptMatchingPairResponse> MatchingPairs);

public sealed record QuizAttemptOptionResponse(
    Guid PublicId,
    string OptionText,
    string? OptionPinyin,
    short SortOrder);

public sealed record QuizAttemptMatchingPairResponse(
    Guid PublicId,
    string LeftText,
    string? LeftPinyin,
    short SortOrder);
