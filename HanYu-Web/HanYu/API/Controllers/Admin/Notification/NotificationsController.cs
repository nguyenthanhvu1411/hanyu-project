using HanYu.Application.Features.Notification.Admin.Notifications;
using HanYu.Application.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Notification;

[ApiController]
[Route("api/v1/admin/notifications")]
[Authorize(Roles = "Admin")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationAdminService _service;

    public NotificationsController(INotificationAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AdminNotificationQuery query, CancellationToken ct)
    {
        var result = await _service.GetNotificationsAsync(query, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await _service.GetNotificationAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request, CancellationToken ct)
    {
        var result = await _service.SendAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastNotificationRequest request, CancellationToken ct)
    {
        var result = await _service.BroadcastAsync(request, ct);
        return result.IsSuccess ? Ok(new { SentCount = result.Value }) : BadRequest(result.Error);
    }
}
