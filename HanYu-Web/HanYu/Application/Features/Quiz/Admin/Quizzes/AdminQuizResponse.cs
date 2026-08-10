using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Quizzes;

public sealed record AdminQuizResponse(
    long Id,
    Guid PublicId,
    long? LessonId,
    Guid? LessonPublicId,
    string? LessonTitleVi,
    string TitleVi,
    string? DescriptionVi,
    QuizType QuizType,
    decimal PassingScore,
    int? TimeLimitSeconds,
    int MaxAttempts,
    QuizShuffleMode ShuffleMode,
    QuizFeedbackMode FeedbackMode,
    bool AllowRetry,
    bool ShowCorrectAnswer,
    bool ShowExplanation,
    ContentStatus Status,
    int Version,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
