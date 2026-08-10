using HanYu.Application.Common.Models;
using HanYu.Application.Features.Notification.Public.Notifications;
using HanYu.Application.Features.Notification.Public.Preferences;
using HanYu.Application.Interfaces.Notification;
using HanYu.Domain.Entities.Notification;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Notification;

public sealed class NotificationPublicService : INotificationPublicService
{
    private readonly HanYuDbContext _db;

    public NotificationPublicService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<NotificationResponse>>> GetMyNotificationsAsync(
        Guid userId,
        NotificationQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _db.Set<InAppNotification>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        var now = DateTimeOffset.UtcNow;

        if (!query.IncludeExpired)
        {
            source = source.Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > now);
        }

        if (query.Type.HasValue)
        {
            source = source.Where(x => x.Type == query.Type.Value);
        }

        if (query.IsRead.HasValue)
        {
            source = query.IsRead.Value
                ? source.Where(x => x.ReadAt.HasValue)
                : source.Where(x => !x.ReadAt.HasValue);
        }

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

        var items = values.Select(x => new NotificationResponse(
            x.PublicId,
            x.Type,
            x.Title,
            x.Message,
            x.ActionUrl,
            x.MetadataJson,
            x.ReadAt.HasValue,
            x.ExpiresAt.HasValue && x.ExpiresAt <= now,
            x.CreatedAt,
            x.ReadAt,
            x.ExpiresAt))
            .ToArray();

        return Result.Success(new PagedResult<NotificationResponse>(
            items,
            query.NormalizedPage,
            query.NormalizedPageSize,
            total));
    }

    public async Task<Result<int>> GetUnreadCountAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var count = await _db.Set<InAppNotification>()
            .CountAsync(x => x.UserId == userId && !x.ReadAt.HasValue && (!x.ExpiresAt.HasValue || x.ExpiresAt > now), cancellationToken);

        return Result.Success(count);
    }

    public async Task<Result> MarkReadAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindOwnedAsync(userId, publicId, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("Notification.NotFound", "Không tìm thấy notification."));
        }

        entity.MarkRead();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> MarkUnreadAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindOwnedAsync(userId, publicId, cancellationToken);

        if (entity is null)
        {
            return Result.Failure(Error.NotFound("Notification.NotFound", "Không tìm thấy notification."));
        }

        entity.MarkUnread();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> MarkAllReadAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        var values = await _db.Set<InAppNotification>()
            .Where(x => x.UserId == userId && !x.ReadAt.HasValue && (!x.ExpiresAt.HasValue || x.ExpiresAt > now))
            .ToArrayAsync(cancellationToken);

        foreach (var item in values)
            item.MarkRead();

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<NotificationPreferenceResponse>> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetOrCreatePreferenceAsync(userId, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success(MapPreference(entity));
    }

    public async Task<Result<NotificationPreferenceResponse>> UpdatePreferencesAsync(
        Guid userId,
        UpdateNotificationPreferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetOrCreatePreferenceAsync(userId, cancellationToken);

        entity.UpdateChannels(request.InAppEnabled, request.EmailEnabled);
        entity.UpdateLearningReminder(request.LearningReminderEnabled, request.PreferredReminderTime, request.Timezone);
        entity.UpdateReviewReminder(request.ReviewReminderEnabled);
        entity.UpdateSecurityNotifications(request.SecurityNotificationEnabled);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success(MapPreference(entity));
    }

    private async Task<InAppNotification?> FindOwnedAsync(
        Guid userId,
        Guid publicId,
        CancellationToken cancellationToken)
        => await _db.Set<InAppNotification>()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == publicId, cancellationToken);

    private async Task<NotificationPreference> GetOrCreatePreferenceAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await _db.Set<NotificationPreference>()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (entity is not null)
            return entity;

        entity = new NotificationPreference(userId);
        _db.Add(entity);
        return entity;
    }

    private static NotificationPreferenceResponse MapPreference(NotificationPreference entity)
        => new(
            entity.InAppEnabled,
            entity.EmailEnabled,
            entity.LearningReminderEnabled,
            entity.ReviewReminderEnabled,
            entity.SecurityNotificationEnabled,
            entity.PreferredReminderTime,
            entity.Timezone,
            entity.UpdatedAt);
}
