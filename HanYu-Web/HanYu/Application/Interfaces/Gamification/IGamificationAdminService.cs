using HanYu.Application.Common.Models;
using HanYu.Application.Features.Gamification.Admin.Achievements;
using HanYu.Application.Features.Gamification.Admin.Xp;
using HanYu.Application.Features.Gamification.Public.Profile;

namespace HanYu.Application.Interfaces.Gamification;

public interface IGamificationAdminService
{
    Task<Result<IReadOnlyCollection<AdminAchievementResponse>>> GetAchievementsAsync(
        CancellationToken cancellationToken = default);

    Task<Result<AdminAchievementResponse>> CreateAchievementAsync(
        CreateAchievementRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<AdminAchievementResponse>> UpdateAchievementAsync(
        long id,
        UpdateAchievementRequest request,
        CancellationToken cancellationToken = default);

    Task<Result> ActivateAchievementAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeactivateAchievementAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAchievementAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<AdminXpTransactionResponse>>> GetXpTransactionsAsync(
        AdminXpQuery query,
        CancellationToken cancellationToken = default);

    Task<Result> AdjustXpAsync(
        Guid userId,
        AdjustXpRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GamificationProfileResponse>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
