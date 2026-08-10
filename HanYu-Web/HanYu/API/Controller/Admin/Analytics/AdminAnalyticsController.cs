using HanYu.API.Common.Extensions;
using HanYu.Application.Features.Analytics.Admin.Users;
using HanYu.Application.Interfaces.Analytics;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controller.Admin.Analytics;

[ApiController]
[Authorize(Policy = Policies.AdminOnly)]
[Route("api/v1/admin")]
public sealed class AdminAnalyticsController : ControllerBase
{
    private readonly IAnalyticsAdminService _service;

    public AdminAnalyticsController(IAnalyticsAdminService service)
    {
        _service = service;
    }

    [HttpGet("analytics/dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetDashboardAsync(cancellationToken));

    [HttpGet("analytics/daily")]
    public async Task<IActionResult> GetDailyStats(
        [FromQuery] AdminLearningStatQuery query,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetDailyStatsAsync(query, cancellationToken));

    [HttpGet("users/{userId:guid}/analytics")]
    public async Task<IActionResult> GetUserSummary(
        Guid userId,
        CancellationToken cancellationToken)
        => this.ToActionResult(
            await _service.GetUserSummaryAsync(userId, cancellationToken));
}
