using HanYu.API.Common.Extensions;
using HanYu.Application.Interfaces.Analytics;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Public.Analytics;

[ApiController]
[Authorize]
[Route("api/v1/public/analytics/me")]
public sealed class PublicAnalyticsController : ControllerBase
{
    private readonly IAnalyticsPublicService _service;
    private readonly ICurrentUserService _currentUser;

    public PublicAnalyticsController(
        IAnalyticsPublicService service,
        ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetMySummaryAsync(
                _currentUser.UserId.Value,
                cancellationToken));

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyStats(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetMyStatsAsync(
                _currentUser.UserId.Value,
                from,
                to,
                cancellationToken));
}
