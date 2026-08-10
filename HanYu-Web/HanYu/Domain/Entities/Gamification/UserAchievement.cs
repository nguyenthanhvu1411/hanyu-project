namespace HanYu.Domain.Entities.Gamification;

public class UserAchievement
{
    public Guid UserId { get; private set; }

    public long AchievementId { get; private set; }

    public DateTimeOffset UnlockedAt { get; private set; }
        = DateTimeOffset.UtcNow;

    protected UserAchievement()
    {
    }

    public UserAchievement(
        Guid userId,
        long achievementId,
        DateTimeOffset? unlockedAt = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId không hợp lệ.",
                nameof(userId));

        if (achievementId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(achievementId));

        UserId = userId;
        AchievementId = achievementId;

        UnlockedAt =
            unlockedAt ?? DateTimeOffset.UtcNow;
    }
}