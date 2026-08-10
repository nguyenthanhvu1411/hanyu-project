using HanYu.Application.Common.Models;
using HanYu.Application.Features.Gamification.Admin.Achievements;
using HanYu.Application.Features.Gamification.Admin.Xp;
using HanYu.Application.Features.Gamification.Public.Profile;
using HanYu.Application.Interfaces.Gamification;
using HanYu.Domain.Constants;
using HanYu.Domain.Entities.Gamification;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Gamification;

public sealed class GamificationAdminService : IGamificationAdminService
{
    private readonly HanYuDbContext _db;
    private readonly IGamificationService _publicService;

    public GamificationAdminService(HanYuDbContext db, IGamificationService publicService)
    {
        _db = db;
        _publicService = publicService;
    }

    public async Task<Result<IReadOnlyCollection<AdminAchievementResponse>>> GetAchievementsAsync(
        CancellationToken cancellationToken = default)
    {
        var achievements = await _db.Set<Achievement>()
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToArrayAsync(cancellationToken);

        var result = achievements.Select(x => new AdminAchievementResponse(
            x.Id,
            x.PublicId,
            x.Code,
            x.NameVi,
            x.DescriptionVi,
            x.IconUrl,
            x.XpReward,
            x.IsActive,
            x.SortOrder,
            x.CreatedAt,
            x.UpdatedAt)).ToArray();

        return Result.Success<IReadOnlyCollection<AdminAchievementResponse>>(result);
    }

    public async Task<Result<AdminAchievementResponse>> CreateAchievementAsync(
        CreateAchievementRequest request,
        CancellationToken cancellationToken = default)
    {
        var exists = await _db.Set<Achievement>()
            .AnyAsync(x => x.Code == request.Code, cancellationToken);
            
        if (exists)
        {
            return Result.Failure<AdminAchievementResponse>(Error.Conflict("Achievement.CodeExists", "Mã code đã tồn tại."));
        }

        var entity = new Achievement(
            request.Code,
            request.NameVi,
            request.XpReward,
            request.SortOrder);
            
        entity.Update(
            request.Code,
            request.NameVi,
            request.DescriptionVi,
            request.IconUrl,
            request.XpReward,
            request.SortOrder);

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AdminAchievementResponse(
            entity.Id,
            entity.PublicId,
            entity.Code,
            entity.NameVi,
            entity.DescriptionVi,
            entity.IconUrl,
            entity.XpReward,
            entity.IsActive,
            entity.SortOrder,
            entity.CreatedAt,
            entity.UpdatedAt));
    }

    public async Task<Result<AdminAchievementResponse>> UpdateAchievementAsync(
        long id,
        UpdateAchievementRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Achievement>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure<AdminAchievementResponse>(Error.NotFound("Achievement.NotFound", "Không tìm thấy Achievement."));

        if (entity.Code != request.Code)
        {
            var exists = await _db.Set<Achievement>()
                .AnyAsync(x => x.Code == request.Code && x.Id != id, cancellationToken);
            
            if (exists)
            {
                return Result.Failure<AdminAchievementResponse>(Error.Conflict("Achievement.CodeExists", "Mã code đã tồn tại."));
            }
        }

        entity.Update(
            request.Code,
            request.NameVi,
            request.DescriptionVi,
            request.IconUrl,
            request.XpReward,
            request.SortOrder);

        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(new AdminAchievementResponse(
            entity.Id,
            entity.PublicId,
            entity.Code,
            entity.NameVi,
            entity.DescriptionVi,
            entity.IconUrl,
            entity.XpReward,
            entity.IsActive,
            entity.SortOrder,
            entity.CreatedAt,
            entity.UpdatedAt));
    }

    public async Task<Result> ActivateAchievementAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Achievement>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound("Achievement.NotFound", "Không tìm thấy Achievement."));

        entity.Activate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeactivateAchievementAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Achievement>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound("Achievement.NotFound", "Không tìm thấy Achievement."));

        entity.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAchievementAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Achievement>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("Achievement.NotFound", "Không tìm thấy Achievement."));
        }

        var used = await _db.Set<UserAchievement>().AnyAsync(x => x.AchievementId == id, cancellationToken);

        if (used)
        {
            return Result.Failure(Error.Conflict("Achievement.InUse", "Achievement đã được người dùng mở khóa. Hãy Deactivate thay vì xóa."));
        }

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<PagedResult<AdminXpTransactionResponse>>> GetXpTransactionsAsync(
        AdminXpQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _db.Set<XpTransaction>().AsNoTracking();

        if (query.UserId.HasValue)
        {
            source = source.Where(x => x.UserId == query.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SourceType))
        {
            source = source.Where(x => x.SourceType == query.SourceType);
        }
        
        if (query.IsCredit.HasValue)
        {
            source = query.IsCredit.Value
                ? source.Where(x => x.Amount > 0)
                : source.Where(x => x.Amount < 0);
        }

        if (query.From.HasValue)
            source = source.Where(x => x.CreatedAt >= query.From.Value);

        if (query.To.HasValue)
            source = source.Where(x => x.CreatedAt <= query.To.Value);

        source = query.Sort?.ToLowerInvariant() switch
        {
            "createdat" => source.OrderBy(x => x.CreatedAt),
            _ => source.OrderByDescending(x => x.CreatedAt)
        };

        var total = await source.LongCountAsync(cancellationToken);
        var values = await source
            .Skip((query.NormalizedPage - 1) * query.NormalizedPageSize)
            .Take(query.NormalizedPageSize)
            .ToArrayAsync(cancellationToken);

        var items = values.Select(x => new AdminXpTransactionResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.Amount,
            x.Reason,
            x.SourceType,
            x.SourceId,
            x.CreatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminXpTransactionResponse>(items, query.NormalizedPage, query.NormalizedPageSize, total));
    }

    public async Task<Result> AdjustXpAsync(
        Guid userId,
        AdjustXpRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Amount == 0 || Math.Abs(request.Amount) > GamificationConstants.MaxManualXpAdjustment)
        {
            return Result.Failure(Error.Validation("Gamification.InvalidXpAdjustment", "XP adjustment không hợp lệ."));
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return Result.Failure(Error.Validation("Gamification.ReasonRequired", "Phải nhập lý do điều chỉnh XP."));
        }

        _db.Add(new XpTransaction(
            userId,
            request.Amount,
            request.Reason,
            XpSources.AdminAdjustment,
            Guid.NewGuid().ToString("N")));

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<GamificationProfileResponse>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _publicService.GetProfileAsync(userId, cancellationToken);
    }
}
