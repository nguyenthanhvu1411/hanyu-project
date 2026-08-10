using HanYu.Domain.Entities;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Enums;
using HanYu.Domain.Constants;

namespace HanYu.Domain.Entities.Learning;

public class UserLearningGoal : TimestampedEntity
{
    public Guid UserId { get; private set; }

    public short TargetHskLevel { get; private set; }

    public DateOnly? TargetDate { get; private set; }

    public short DailyGoalMinutes { get; private set; } = LearningConstants.DefaultDailyGoalMinutes;

    public short? DailyVocabularyGoal { get; private set; }

    public short? WeeklyLessonGoal { get; private set; }

    public LearningGoalStatus Status { get; private set; }
        = LearningGoalStatus.Active;

    public DateTimeOffset StartedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? PausedAt { get; private set; }

    public User User { get; private set; } = null!;

    protected UserLearningGoal()
    {
    }

    public UserLearningGoal(
        Guid userId,
        short targetHskLevel,
        short dailyGoalMinutes = LearningConstants.DefaultDailyGoalMinutes,
        DateOnly? targetDate = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        UserId = userId;

        Update(
            targetHskLevel,
            targetDate,
            dailyGoalMinutes,
            null,
            null);
    }

    public void Update(
        short targetHskLevel,
        DateOnly? targetDate,
        short dailyGoalMinutes,
        short? dailyVocabularyGoal,
        short? weeklyLessonGoal)
    {
        EnsureEditable();

        if (targetHskLevel < LearningConstants.MinHskLevel || targetHskLevel > LearningConstants.MaxHskLevel)
            throw new ArgumentOutOfRangeException(
                nameof(targetHskLevel),
                $"HSK phải từ {LearningConstants.MinHskLevel} đến {LearningConstants.MaxHskLevel}.");

        if (dailyGoalMinutes < LearningConstants.MinDailyGoalMinutes || dailyGoalMinutes > LearningConstants.MaxDailyGoalMinutes)
            throw new ArgumentOutOfRangeException(
                nameof(dailyGoalMinutes),
                $"Daily goal phải từ {LearningConstants.MinDailyGoalMinutes} đến {LearningConstants.MaxDailyGoalMinutes} phút.");

        if (dailyVocabularyGoal.HasValue &&
            dailyVocabularyGoal.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyVocabularyGoal));
        }

        if (weeklyLessonGoal.HasValue &&
            weeklyLessonGoal.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weeklyLessonGoal));
        }

        TargetHskLevel = targetHskLevel;
        TargetDate = targetDate;
        DailyGoalMinutes = dailyGoalMinutes;
        DailyVocabularyGoal = dailyVocabularyGoal;
        WeeklyLessonGoal = weeklyLessonGoal;

        MarkUpdated();
    }

    public void Pause()
    {
        if (Status == LearningGoalStatus.Paused)
            return;

        if (Status != LearningGoalStatus.Active)
            throw new InvalidOperationException(
                "Chỉ learning goal Active mới có thể Pause.");

        Status = LearningGoalStatus.Paused;
        PausedAt = DateTimeOffset.UtcNow;

        MarkUpdated();
    }

    public void Resume()
    {
        if (Status == LearningGoalStatus.Active)
            return;

        if (Status != LearningGoalStatus.Paused)
            throw new InvalidOperationException(
                "Chỉ learning goal Paused mới có thể Resume.");

        Status = LearningGoalStatus.Active;
        PausedAt = null;

        MarkUpdated();
    }

    public void Complete()
    {
        if (Status == LearningGoalStatus.Completed)
            return;

        if (Status == LearningGoalStatus.Cancelled)
            throw new InvalidOperationException(
                "Learning goal đã bị Cancelled.");

        Status = LearningGoalStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        PausedAt = null;

        MarkUpdated();
    }

    public void Cancel()
    {
        if (Status == LearningGoalStatus.Cancelled)
            return;

        if (Status == LearningGoalStatus.Completed)
            throw new InvalidOperationException(
                "Learning goal đã Completed.");

        Status = LearningGoalStatus.Cancelled;
        PausedAt = null;

        MarkUpdated();
    }

    private void EnsureEditable()
    {
        if (Status is LearningGoalStatus.Completed or
            LearningGoalStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Learning goal đã kết thúc, không thể cập nhật.");
        }
    }
}
