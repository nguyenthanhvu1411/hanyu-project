namespace HanYu.Domain.Entities.Analytics;

public class UserStreak
{
    public Guid UserId { get; private set; }

    public int CurrentStreak { get; private set; }

    public int LongestStreak { get; private set; }

    public DateOnly? LastLearningDate { get; private set; }

    public DateOnly? CurrentStreakStartedAt { get; private set; }

    public int TotalActiveDays { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected UserStreak()
    {
    }

    public UserStreak(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));
        }

        UserId = userId;
    }

    public bool RegisterLearningDay(
        DateOnly learningDate)
    {
        if (!LastLearningDate.HasValue)
        {
            CurrentStreak = 1;
            LongestStreak = 1;

            LastLearningDate = learningDate;
            CurrentStreakStartedAt = learningDate;

            TotalActiveDays = 1;

            MarkUpdated();

            return true;
        }

        if (learningDate == LastLearningDate.Value)
        {
            return false;
        }

        if (learningDate < LastLearningDate.Value)
        {
            return false;
        }

        var previousDay =
            learningDate.AddDays(-1);

        if (LastLearningDate.Value == previousDay)
        {
            CurrentStreak++;
        }
        else
        {
            CurrentStreak = 1;
            CurrentStreakStartedAt = learningDate;
        }

        LastLearningDate = learningDate;
        TotalActiveDays++;

        if (CurrentStreak > LongestStreak)
            LongestStreak = CurrentStreak;

        MarkUpdated();

        return true;
    }

    public void Recompute(
        int currentStreak,
        int longestStreak,
        int totalActiveDays,
        DateOnly? lastLearningDate,
        DateOnly? currentStreakStartedAt)
    {
        if (currentStreak < 0 ||
            longestStreak < 0 ||
            totalActiveDays < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (longestStreak < currentStreak)
        {
            throw new ArgumentException(
                "LongestStreak không thể nhỏ hơn CurrentStreak.");
        }

        CurrentStreak = currentStreak;
        LongestStreak = longestStreak;
        TotalActiveDays = totalActiveDays;

        LastLearningDate = lastLearningDate;
        CurrentStreakStartedAt =
            currentStreakStartedAt;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt =
            DateTimeOffset.UtcNow;
    }
}