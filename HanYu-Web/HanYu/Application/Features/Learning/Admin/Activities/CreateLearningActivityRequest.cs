using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Learning.Admin.Activities;

public sealed record CreateLearningActivityRequest(
    Guid UserId,
    LearningActivityType ActivityType,
    long? LessonId,
    long? VocabularyId,
    long? QuizAttemptId,
    long? FlashcardSessionId,
    int DurationSeconds,
    int XpEarned,
    bool IsCompleted,
    string? MetadataJson);
