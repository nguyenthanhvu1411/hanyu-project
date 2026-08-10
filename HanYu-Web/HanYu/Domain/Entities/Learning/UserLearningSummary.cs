namespace HanYu.Domain.Entities.Learning;

public class UserLearningSummary
{
    public Guid UserId { get; private set; }

    public int TotalLearningSeconds { get; private set; }

    public int TotalLessonsCompleted { get; private set; }

    public int TotalVocabularyLearned { get; private set; }

    public int TotalVocabularyMastered { get; private set; }

    public int TotalReviews { get; private set; }

    public int TotalQuizAttempts { get; private set; }

    public int TotalQuizPassed { get; private set; }

    public int TotalXp { get; private set; }

    public short CurrentHskLevel { get; private set; } = 1;

    public decimal OverallMasteryPercent { get; private set; }

    public DateTimeOffset? LastLearningAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected UserLearningSummary()
    {
    }

    public UserLearningSummary(
        Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;
    }

    public void Update(
        int totalLearningSeconds,
        int totalLessonsCompleted,
        int totalVocabularyLearned,
        int totalVocabularyMastered,
        int totalReviews,
        int totalQuizAttempts,
        int totalQuizPassed,
        int totalXp,
        short currentHskLevel,
        decimal overallMasteryPercent,
        DateTimeOffset? lastLearningAt)
    {
        ValidateNonNegative(
            totalLearningSeconds,
            nameof(totalLearningSeconds));

        ValidateNonNegative(
            totalLessonsCompleted,
            nameof(totalLessonsCompleted));

        ValidateNonNegative(
            totalVocabularyLearned,
            nameof(totalVocabularyLearned));

        ValidateNonNegative(
            totalVocabularyMastered,
            nameof(totalVocabularyMastered));

        ValidateNonNegative(
            totalReviews,
            nameof(totalReviews));

        ValidateNonNegative(
            totalQuizAttempts,
            nameof(totalQuizAttempts));

        ValidateNonNegative(
            totalQuizPassed,
            nameof(totalQuizPassed));

        ValidateNonNegative(
            totalXp,
            nameof(totalXp));

        if (currentHskLevel < 1 ||
            currentHskLevel > 6)
        {
            throw new ArgumentOutOfRangeException(
                nameof(currentHskLevel));
        }

        if (overallMasteryPercent < 0 ||
            overallMasteryPercent > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(overallMasteryPercent));
        }

        if (totalQuizPassed >
            totalQuizAttempts)
        {
            throw new ArgumentException(
                "TotalQuizPassed không được lớn hơn TotalQuizAttempts.");
        }

        if (totalVocabularyMastered >
            totalVocabularyLearned)
        {
            throw new ArgumentException(
                "TotalVocabularyMastered không được lớn hơn TotalVocabularyLearned.");
        }

        TotalLearningSeconds =
            totalLearningSeconds;

        TotalLessonsCompleted =
            totalLessonsCompleted;

        TotalVocabularyLearned =
            totalVocabularyLearned;

        TotalVocabularyMastered =
            totalVocabularyMastered;

        TotalReviews =
            totalReviews;

        TotalQuizAttempts =
            totalQuizAttempts;

        TotalQuizPassed =
            totalQuizPassed;

        TotalXp =
            totalXp;

        CurrentHskLevel =
            currentHskLevel;

        OverallMasteryPercent =
            overallMasteryPercent;

        LastLearningAt =
            lastLearningAt;

        UpdatedAt =
            DateTimeOffset.UtcNow;
    }

    private static void ValidateNonNegative(
        int value,
        string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(
                parameterName);
    }
}
