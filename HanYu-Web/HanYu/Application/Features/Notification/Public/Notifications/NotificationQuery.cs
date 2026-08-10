using HanYu.Application.Common.Models;
using HanYu.Domain.Enums;

namespace HanYu.Application.Features.Notification.Public.Notifications;

public sealed record NotificationQuery : PaginationRequest
{
    public NotificationType? Type { get; init; }

    public bool? IsRead { get; init; }

    public bool IncludeExpired { get; init; }

    public string? Sort { get; init; } = "-createdAt";
}
