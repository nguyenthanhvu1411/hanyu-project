using HanYu.Application.Common.Models;
using HanYu.Application.Features.Notification.Admin.Deliveries;
using HanYu.Application.Features.Notification.Admin.Notifications;
using HanYu.Application.Interfaces.Notification;
using HanYu.Domain.Entities.Notification;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Notification;

public sealed class NotificationAdminService : INotificationAdminService
{
    private readonly HanYuDbContext _db;

    public NotificationAdminService(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task<Result<PagedResult<AdminNotificationResponse>>> GetNotificationsAsync(
        AdminNotificationQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _db.Set<InAppNotification>().AsNoTracking();

        if (query.UserId.HasValue)
        {
            source = source.Where(x => x.UserId == query.UserId.Value);
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

        var now = DateTimeOffset.UtcNow;
        if (query.IsExpired.HasValue)
        {
            source = query.IsExpired.Value
                ? source.Where(x => x.ExpiresAt.HasValue && x.ExpiresAt <= now)
                : source.Where(x => !x.ExpiresAt.HasValue || x.ExpiresAt > now);
        }

        if (query.From.HasValue)
        {
            source = source.Where(x => x.CreatedAt >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            source = source.Where(x => x.CreatedAt <= query.To.Value);
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

        var items = values.Select(x => new AdminNotificationResponse(
            x.Id,
            x.PublicId,
            x.UserId,
            x.Type,
            x.Title,
            x.Message,
            x.ActionUrl,
            x.MetadataJson,
            x.ReadAt.HasValue,
            x.ExpiresAt.HasValue && x.ExpiresAt <= now,
            x.CreatedAt,
            x.ReadAt,
            x.ExpiresAt)).ToArray();

        return Result.Success(new PagedResult<AdminNotificationResponse>(items, query.NormalizedPage, query.NormalizedPageSize, total));
    }

    public async Task<Result<AdminNotificationResponse>> GetNotificationAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<InAppNotification>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure<AdminNotificationResponse>(Error.NotFound("Notification.NotFound", "Không tìm thấy notification."));
        
        var now = DateTimeOffset.UtcNow;
        return Result.Success(new AdminNotificationResponse(
            entity.Id,
            entity.PublicId,
            entity.UserId,
            entity.Type,
            entity.Title,
            entity.Message,
            entity.ActionUrl,
            entity.MetadataJson,
            entity.ReadAt.HasValue,
            entity.ExpiresAt.HasValue && entity.ExpiresAt <= now,
            entity.CreatedAt,
            entity.ReadAt,
            entity.ExpiresAt));
    }

    public async Task<Result<AdminNotificationResponse>> SendAsync(
        SendNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = new InAppNotification(
            request.UserId,
            request.Type,
            request.Title,
            request.Message,
            request.ActionUrl,
            request.MetadataJson,
            request.ExpiresAt);

        _db.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);
        
        return await GetNotificationAsync(notification.Id, cancellationToken);
    }

    public async Task<Result<int>> BroadcastAsync(
        BroadcastNotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIds = request.UserIds?.ToArray() ?? Array.Empty<Guid>();
        
        if (userIds.Length == 0)
        {
            userIds = await _db.Set<Domain.Entities.Identity.User>()
                .Where(x => !x.DeletedAt.HasValue)
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);
        }

        var notifications = userIds.Select(id => new InAppNotification(
            id,
            request.Type,
            request.Title,
            request.Message,
            request.ActionUrl,
            request.MetadataJson,
            request.ExpiresAt)).ToList();

        _db.AddRange(notifications);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success(notifications.Count);
    }

    public async Task<Result<PagedResult<AdminNotificationDeliveryResponse>>> GetDeliveriesAsync(
        AdminNotificationDeliveryQuery query,
        CancellationToken cancellationToken = default)
    {
        var source = _db.Set<NotificationDelivery>().AsNoTracking();

        if (query.NotificationId.HasValue)
            source = source.Where(x => x.NotificationId == query.NotificationId.Value);

        if (query.Channel.HasValue)
            source = source.Where(x => x.Channel == query.Channel.Value);

        if (query.Status.HasValue)
            source = source.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Provider))
            source = source.Where(x => x.Provider == query.Provider);

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

        var notificationIds = values.Select(x => x.NotificationId).Distinct().ToArray();
        var publicIds = await _db.Set<InAppNotification>()
            .Where(x => notificationIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.PublicId, cancellationToken);

        var items = values.Select(x => new AdminNotificationDeliveryResponse(
            x.Id,
            x.PublicId,
            x.NotificationId,
            publicIds.GetValueOrDefault(x.NotificationId),
            x.Channel,
            x.Status,
            x.Destination,
            x.Provider,
            x.ProviderMessageId,
            x.AttemptCount,
            x.LastAttemptAt,
            x.SentAt,
            x.DeliveredAt,
            x.FailedAt,
            x.FailureCode,
            x.FailureReason,
            x.CreatedAt)).ToArray();

        return Result.Success(new PagedResult<AdminNotificationDeliveryResponse>(items, query.NormalizedPage, query.NormalizedPageSize, total));
    }

    public async Task<Result> RetryDeliveryAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<NotificationDelivery>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound("Delivery.NotFound", "Không tìm thấy delivery."));
        
        entity.StartAttempt();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelDeliveryAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<NotificationDelivery>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return Result.Failure(Error.NotFound("Delivery.NotFound", "Không tìm thấy delivery."));
        
        entity.Cancel();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
