using HanYu.Application.Features.Operations.Public.Events;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Operations;

[ApiController]
[Route("api/v1/public/events")]
public sealed class ProductEventsController : ControllerBase
{
    private readonly IProductEventTracker _tracker;
    private readonly ICurrentUserService _currentUser;

    public ProductEventsController(
        IProductEventTracker tracker,
        ICurrentUserService currentUser)
    {
        _tracker = tracker;
        _currentUser = currentUser;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Track(
        [FromBody] TrackProductEventRequest request,
        CancellationToken cancellationToken)
    {
        await _tracker.TrackAsync(
            _currentUser.UserId != Guid.Empty ? _currentUser.UserId : null,
            request,
            cancellationToken);

        return Accepted();
    }
}
