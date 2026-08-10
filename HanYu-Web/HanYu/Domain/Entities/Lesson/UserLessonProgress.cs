using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;

namespace HanYu.Domain.Entities.Lesson;

public class UserLessonProgress
{
    public Guid UserId { get; private set; }

    public long LessonId { get; private set; }

    public long? LastSectionId { get; private set; }

    public LessonProgressStatus Status { get; private set; }
        = LessonProgressStatus.NotStarted;

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? LastAccessedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public int LastPosition { get; private set; }

    public decimal CompletionPercent { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public User User { get; private set; } = null!;

    public Lesson Lesson { get; private set; } = null!;

    protected UserLessonProgress()
    {
    }

    public UserLessonProgress(
        Guid userId,
        long lessonId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (lessonId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(lessonId));

        UserId = userId;
        LessonId = lessonId;
    }

    public void Start()
    {
        if (Status == LessonProgressStatus.Completed)
            return;

        StartedAt ??= DateTimeOffset.UtcNow;
        LastAccessedAt = DateTimeOffset.UtcNow;

        Status = LessonProgressStatus.InProgress;

        MarkUpdated();
    }

    public void RegisterAccess()
    {
        LastAccessedAt = DateTimeOffset.UtcNow;

        if (Status == LessonProgressStatus.NotStarted)
        {
            StartedAt ??= DateTimeOffset.UtcNow;
            Status = LessonProgressStatus.InProgress;
        }

        MarkUpdated();
    }

    public void UpdateProgress(
        long? lastSectionId,
        int lastPosition,
        decimal completionPercent)
    {
        if (Status == LessonProgressStatus.Completed)
            throw new InvalidOperationException(
                "Lesson đã hoàn thành.");

        if (lastSectionId.HasValue &&
            lastSectionId.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lastSectionId));
        }

        if (lastPosition < 0)
            throw new ArgumentOutOfRangeException(
                nameof(lastPosition));

        if (completionPercent < 0 ||
            completionPercent >= 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(completionPercent),
                "CompletionPercent phải từ 0 đến nhỏ hơn 100. Dùng Complete() để hoàn thành lesson.");
        }

        StartedAt ??= DateTimeOffset.UtcNow;

        LastSectionId = lastSectionId;
        LastPosition = lastPosition;
        CompletionPercent = completionPercent;

        LastAccessedAt = DateTimeOffset.UtcNow;

        if (Status == LessonProgressStatus.NotStarted)
            Status = LessonProgressStatus.InProgress;

        MarkUpdated();
    }

    public void Complete()
    {
        if (Status == LessonProgressStatus.Completed)
            return;

        StartedAt ??= DateTimeOffset.UtcNow;

        Status = LessonProgressStatus.Completed;
        CompletionPercent = 100;

        CompletedAt = DateTimeOffset.UtcNow;
        LastAccessedAt = CompletedAt;

        MarkUpdated();
    }

    public void Reset()
    {
        Status = LessonProgressStatus.NotStarted;

        LastSectionId = null;
        StartedAt = null;
        LastAccessedAt = null;
        CompletedAt = null;

        LastPosition = 0;
        CompletionPercent = 0;

        MarkUpdated();
    }

    private void MarkUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
