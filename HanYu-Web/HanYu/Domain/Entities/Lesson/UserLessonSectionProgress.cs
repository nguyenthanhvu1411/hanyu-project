namespace HanYu.Domain.Entities.Lesson;

public class UserLessonSectionProgress
{
    public Guid UserId { get; private set; }

    public long LessonSectionId { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public int TimeSpentSeconds { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public LessonSection LessonSection { get; private set; } = null!;

    protected UserLessonSectionProgress()
    {
    }

    public UserLessonSectionProgress(
        Guid userId,
        long lessonSectionId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (lessonSectionId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonSectionId));

        UserId = userId;
        LessonSectionId = lessonSectionId;
    }

    public void Start()
    {
        if (IsCompleted)
            return;

        StartedAt ??=
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void AddTimeSpent(int seconds)
    {
        if (seconds <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(seconds));

        if (IsCompleted)
            throw new InvalidOperationException(
                "Lesson section đã hoàn thành.");

        StartedAt ??=
            DateTimeOffset.UtcNow;

        checked
        {
            TimeSpentSeconds += seconds;
        }

        MarkUpdated();
    }

    public void UpdateTimeSpent(
        int totalSeconds)
    {
        if (totalSeconds < 0)
            throw new ArgumentOutOfRangeException(
                nameof(totalSeconds));

        if (IsCompleted)
            throw new InvalidOperationException(
                "Lesson section đã hoàn thành.");

        if (totalSeconds < TimeSpentSeconds)
            throw new InvalidOperationException(
                "TimeSpentSeconds mới không được nhỏ hơn giá trị hiện tại.");

        StartedAt ??=
            DateTimeOffset.UtcNow;

        TimeSpentSeconds =
            totalSeconds;

        MarkUpdated();
    }

    public void Complete()
    {
        if (IsCompleted)
            return;

        StartedAt ??=
            DateTimeOffset.UtcNow;

        IsCompleted = true;

        CompletedAt =
            DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Reopen()
    {
        if (!IsCompleted)
            return;

        IsCompleted = false;
        CompletedAt = null;

        MarkUpdated();
    }

    public void Reset()
    {
        IsCompleted = false;

        StartedAt = null;
        CompletedAt = null;

        TimeSpentSeconds = 0;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt =
            DateTimeOffset.UtcNow;
    }
}
