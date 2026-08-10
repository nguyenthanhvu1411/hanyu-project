namespace HanYu.Application.Features.Gamification.Public.Achievements;

public sealed record AchievementResponse(
    Guid PublicId,
    string Code,
    string NameVi,
    string? DescriptionVi,
    string? IconUrl,
    int XpReward,
    bool IsUnlocked,
    DateTimeOffset? UnlockedAt);
