using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Activities;

public sealed record UpdateLearningActivityRequest(
    LearningActivityType ActivityType,
    long? LessonId,
    long? VocabularyId,
    long? QuizAttemptId,
    long? FlashcardSessionId,
    int DurationSeconds,
    int XpEarned,
    bool IsCompleted,
    string? MetadataJson);
