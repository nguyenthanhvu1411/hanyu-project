using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Public.Quizzes;

public sealed record QuizListItemResponse(
    Guid PublicId,
    string TitleVi,
    string? DescriptionVi,
    QuizType QuizType,
    decimal PassingScore,
    int? TimeLimitSeconds,
    int MaxAttempts,
    int QuestionCount);
