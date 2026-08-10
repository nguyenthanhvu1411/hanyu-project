using HanYu.Application.Interfaces.Notification;
using HanYu.Domain.Entities.Notification;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.Infrastructure.Notification;

public sealed class NotificationSender : INotificationSender
{
    private readonly HanYuDbContext _db;

    public NotificationSender(HanYuDbContext db)
    {
        _db = db;
    }

    public async Task SendAsync(
        Guid userId,
        NotificationType type,
        string title,
        string message,
        string? actionUrl = null,
        string? metadataJson = null,
        DateTimeOffset? expiresAt = null,
        CancellationToken cancellationToken = default)
    {
        var preference = await _db.Set<NotificationPreference>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (preference is not null && !preference.InAppEnabled)
        {
            return;
        }

        var notification = new InAppNotification(
            userId,
            type,
            title,
            message,
            actionUrl,
            metadataJson,
            expiresAt);

        _db.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
