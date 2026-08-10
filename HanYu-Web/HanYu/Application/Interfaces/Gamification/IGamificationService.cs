using HanYu.Application.Common.Models;
using HanYu.Application.Features.Gamification.Public.Achievements;
using HanYu.Application.Features.Gamification.Public.Profile;
using HanYu.Application.Features.Gamification.Public.Xp;

namespace HanYu.Application.Interfaces.Gamification;

public interface IGamificationService
{
    Task<Result<GamificationProfileResponse>> GetProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<AchievementResponse>>> GetAchievementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<XpTransactionResponse>>> GetXpHistoryAsync(
        Guid userId,
        XpHistoryQuery query,
        CancellationToken cancellationToken = default);

    Task<Result> AwardXpAsync(
        Guid userId,
        int amount,
        string reason,
        string sourceType,
        string sourceId,
        CancellationToken cancellationToken = default);

    Task<Result> RegisterLearningActivityAsync(
        Guid userId,
        DateTimeOffset occurredAt,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockAchievementAsync(
        Guid userId,
        string achievementCode,
        CancellationToken cancellationToken = default);
}
