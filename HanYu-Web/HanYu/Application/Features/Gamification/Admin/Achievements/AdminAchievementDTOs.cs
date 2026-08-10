namespace HanYu.Application.Features.Gamification.Admin.Achievements;

public sealed record AdminAchievementResponse(
    long Id,
    Guid PublicId,
    string Code,
    string NameVi,
    string? DescriptionVi,
    string? IconUrl,
    int XpReward,
    bool IsActive,
    int SortOrder,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateAchievementRequest(
    string Code,
    string NameVi,
    string? DescriptionVi,
    string? IconUrl,
    int XpReward,
    int SortOrder);

public sealed record UpdateAchievementRequest(
    string Code,
    string NameVi,
    string? DescriptionVi,
    string? IconUrl,
    int XpReward,
    int SortOrder);
