using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using LessonEntity = HanYu.Domain.Entities.Lesson.Lesson;
using VocabularyEntity = HanYu.Domain.Entities.Vocabulary.Vocabulary;

namespace HanYu.Domain.Entities.Learning;

public class LearningActivity : BaseEntity
{
    public Guid UserId { get; private set; }

    public LearningActivityType ActivityType { get; private set; }

    public long? LessonId { get; private set; }

    public long? VocabularyId { get; private set; }

    public long? QuizAttemptId { get; private set; }

    public long? FlashcardSessionId { get; private set; }

    public int DurationSeconds { get; private set; }

    public int XpEarned { get; private set; }

    public bool IsCompleted { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; private set; }

    public User User { get; private set; } = null!;

    public LessonEntity? Lesson { get; private set; }

    public VocabularyEntity? Vocabulary { get; private set; }

    public QuizAttempt? QuizAttempt { get; private set; }

    public FlashcardSession? FlashcardSession { get; private set; }

    protected LearningActivity()
    {
    }

    public LearningActivity(
        Guid userId,
        LearningActivityType activityType,
        long? lessonId = null,
        long? vocabularyId = null,
        long? quizAttemptId = null,
        long? flashcardSessionId = null,
        string? metadataJson = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        ValidateReferenceId(
            lessonId,
            nameof(lessonId));

        ValidateReferenceId(
            vocabularyId,
            nameof(vocabularyId));

        ValidateReferenceId(
            quizAttemptId,
            nameof(quizAttemptId));

        ValidateReferenceId(
            flashcardSessionId,
            nameof(flashcardSessionId));

        UserId = userId;
        ActivityType = activityType;

        LessonId = lessonId;
        VocabularyId = vocabularyId;
        QuizAttemptId = quizAttemptId;
        FlashcardSessionId = flashcardSessionId;

        MetadataJson = NormalizeMetadata(
            metadataJson);
    }

    public void Update(
        LearningActivityType activityType,
        long? lessonId,
        long? vocabularyId,
        long? quizAttemptId,
        long? flashcardSessionId,
        int durationSeconds,
        int xpEarned,
        string? metadataJson)
    {
        if (durationSeconds < 0)
            throw new ArgumentOutOfRangeException(
                nameof(durationSeconds));

        if (xpEarned < 0)
            throw new ArgumentOutOfRangeException(
                nameof(xpEarned));

        ValidateReferenceId(
            lessonId,
            nameof(lessonId));

        ValidateReferenceId(
            vocabularyId,
            nameof(vocabularyId));

        ValidateReferenceId(
            quizAttemptId,
            nameof(quizAttemptId));

        ValidateReferenceId(
            flashcardSessionId,
            nameof(flashcardSessionId));

        ActivityType = activityType;

        LessonId = lessonId;
        VocabularyId = vocabularyId;
        QuizAttemptId = quizAttemptId;
        FlashcardSessionId = flashcardSessionId;

        DurationSeconds = durationSeconds;
        XpEarned = xpEarned;

        MetadataJson =
            NormalizeMetadata(metadataJson);
    }

    public void AddDuration(
        int seconds)
    {
        if (seconds <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(seconds));

        checked
        {
            DurationSeconds += seconds;
        }
    }

    public void AddXp(
        int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount));

        checked
        {
            XpEarned += amount;
        }
    }

    public void Complete(
        int? durationSeconds = null,
        int? xpEarned = null)
    {
        if (IsCompleted)
            return;

        if (durationSeconds.HasValue)
        {
            if (durationSeconds.Value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(durationSeconds));

            DurationSeconds =
                durationSeconds.Value;
        }

        if (xpEarned.HasValue)
        {
            if (xpEarned.Value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(xpEarned));

            XpEarned =
                xpEarned.Value;
        }

        IsCompleted = true;
        CompletedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateReferenceId(
        long? id,
        string parameterName)
    {
        if (id.HasValue &&
            id.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName);
        }
    }

    private static string? NormalizeMetadata(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
