using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Public.Quizzes;

public sealed record QuizDetailResponse(
    Guid PublicId,
    string TitleVi,
    string? DescriptionVi,
    QuizType QuizType,
    decimal PassingScore,
    int? TimeLimitSeconds,
    int MaxAttempts,
    bool AllowRetry,
    QuizFeedbackMode FeedbackMode,
    int QuestionCount);
