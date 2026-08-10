using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Admin.Notifications;

public sealed record AdminNotificationQuery : PaginationRequest
{
    public Guid? UserId { get; init; }

    public NotificationType? Type { get; init; }

    public bool? IsRead { get; init; }

    public bool? IsExpired { get; init; }

    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Sort { get; init; } = "-createdAt";
}
