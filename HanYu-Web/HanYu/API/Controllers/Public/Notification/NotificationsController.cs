using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Notification.Public.Notifications;
using HanYu.Application.Interfaces.Notification;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Public.Notification;

[ApiController]
[Route("api/v1/public/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public NotificationsController(INotificationPublicService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] NotificationQuery query, CancellationToken ct)
    {
        var result = await _service.GetMyNotificationsAsync(_currentUser.UserId.Value, query, ct);
        return this.ToActionResult(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var result = await _service.GetUnreadCountAsync(_currentUser.UserId.Value, ct);
        return result.IsSuccess ? Ok(new { count = result.Value }) : BadRequest(result.Error);
    }

    [HttpPatch("{publicId:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid publicId, CancellationToken ct)
    {
        var result = await _service.MarkReadAsync(_currentUser.UserId.Value, publicId, ct);
        return this.ToActionResult(result);
    }

    [HttpPatch("{publicId:guid}/unread")]
    public async Task<IActionResult> MarkUnread(Guid publicId, CancellationToken ct)
    {
        var result = await _service.MarkUnreadAsync(_currentUser.UserId.Value, publicId, ct);
        return this.ToActionResult(result);
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var result = await _service.MarkAllReadAsync(_currentUser.UserId.Value, ct);
        return this.ToActionResult(result);
    }
}
