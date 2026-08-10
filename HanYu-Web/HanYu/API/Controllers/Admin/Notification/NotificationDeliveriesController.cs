using HanYu.Application.Features.Notification.Admin.Deliveries;
using HanYu.Application.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Notification;

[ApiController]
[Route("api/v1/admin/notification-deliveries")]
[Authorize(Roles = "Admin")]
public sealed class NotificationDeliveriesController : ControllerBase
{
    private readonly INotificationAdminService _service;

    public NotificationDeliveriesController(INotificationAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AdminNotificationDeliveryQuery query, CancellationToken ct)
    {
        var result = await _service.GetDeliveriesAsync(query, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:long}/retry")]
    public async Task<IActionResult> Retry(long id, CancellationToken ct)
    {
        var result = await _service.RetryDeliveryAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:long}/cancel")]
    public async Task<IActionResult> Cancel(long id, CancellationToken ct)
    {
        var result = await _service.CancelDeliveryAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
