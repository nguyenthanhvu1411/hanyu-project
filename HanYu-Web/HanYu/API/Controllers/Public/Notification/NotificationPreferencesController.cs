using HanYu.Application.Features.Notification.Public.Preferences;
using HanYu.Application.Interfaces.Notification;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Public.Notification;

[ApiController]
[Route("api/v1/public/users/me/notification-preferences")]
[Authorize]
public sealed class NotificationPreferencesController : ControllerBase
{
    private readonly INotificationPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public NotificationPreferencesController(INotificationPublicService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _service.GetPreferencesAsync(_currentUser.UserId.Value, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateNotificationPreferenceRequest request, CancellationToken ct)
    {
        var result = await _service.UpdatePreferencesAsync(_currentUser.UserId.Value, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
