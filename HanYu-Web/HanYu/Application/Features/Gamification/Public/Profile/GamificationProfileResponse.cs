namespace HanYu.Application.Features.Gamification.Public.Profile;

public sealed record GamificationProfileResponse(
    int TotalXp,
    int CurrentStreak,
    int LongestStreak,
    int TotalActiveDays,
    DateOnly? LastLearningDate,
    int AchievementCount);
