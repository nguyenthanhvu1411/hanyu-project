using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Quiz.Admin.Quizzes;

public sealed record UpdateQuizRequest(
    string TitleVi,
    string? DescriptionVi,
    QuizType QuizType,
    decimal PassingScore,
    int? TimeLimitSeconds,
    int MaxAttempts,
    long? LessonId,
    QuizShuffleMode ShuffleMode,
    QuizFeedbackMode FeedbackMode,
    bool AllowRetry,
    bool ShowCorrectAnswer,
    bool ShowExplanation,
    int Version);
