namespace HanYu.Domain.Entities.Analytics;

public class DailyLearningStat
{
    public Guid UserId { get; private set; }

    public DateOnly StatDate { get; private set; }

    public int LearningSeconds { get; private set; }

    public int LessonsStarted { get; private set; }

    public int LessonsCompleted { get; private set; }

    public int VocabularyReviewed { get; private set; }

    public int VocabularyLearned { get; private set; }

    public int CorrectReviews { get; private set; }

    public int WrongReviews { get; private set; }

    public int QuizAttempts { get; private set; }

    public int QuizPassed { get; private set; }

    public int AiInteractions { get; private set; }

    public int XpEarned { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected DailyLearningStat()
    {
    }

    public DailyLearningStat(
        Guid userId,
        DateOnly statDate)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;
        StatDate = statDate;
    }

    public void AddLearningTime(int seconds)
    {
        if (seconds <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(seconds),
                "Thời gian học phải lớn hơn 0.");

        LearningSeconds += seconds;

        MarkUpdated();
    }

    public void RegisterLessonStarted()
    {
        LessonsStarted++;

        MarkUpdated();
    }

    public void RegisterLessonCompleted()
    {
        LessonsCompleted++;

        MarkUpdated();
    }

    public void RegisterVocabularyReviewed(bool wasCorrect)
    {
        VocabularyReviewed++;

        if (wasCorrect)
            CorrectReviews++;
        else
            WrongReviews++;

        MarkUpdated();
    }

    public void RegisterVocabularyLearned()
    {
        VocabularyLearned++;

        MarkUpdated();
    }

    public void RegisterQuizAttempt(bool passed)
    {
        QuizAttempts++;

        if (passed)
            QuizPassed++;

        MarkUpdated();
    }

    public void RegisterAiInteraction()
    {
        AiInteractions++;

        MarkUpdated();
    }

    public void AddXp(int xp)
    {
        if (xp <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(xp),
                "XP phải lớn hơn 0.");

        XpEarned += xp;

        MarkUpdated();
    }

    public void Reset()
    {
        LearningSeconds = 0;
        LessonsStarted = 0;
        LessonsCompleted = 0;
        VocabularyReviewed = 0;
        VocabularyLearned = 0;
        CorrectReviews = 0;
        WrongReviews = 0;
        QuizAttempts = 0;
        QuizPassed = 0;
        AiInteractions = 0;
        XpEarned = 0;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}