using HanYu.Application.Features.Gamification.Public.Xp;
using HanYu.Application.Interfaces.Gamification;
using HanYu.Application.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Public.Gamification;

[ApiController]
[Route("api/v1/public/gamification/me")]
[Authorize]
public sealed class GamificationController : ControllerBase
{
    private readonly IGamificationService _service;
    private readonly ICurrentUserService _currentUser;

    public GamificationController(IGamificationService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await _service.GetProfileAsync(_currentUser.UserId.Value, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("achievements")]
    public async Task<IActionResult> GetAchievements(CancellationToken ct)
    {
        var result = await _service.GetAchievementsAsync(_currentUser.UserId.Value, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("xp-history")]
    public async Task<IActionResult> GetXpHistory([FromQuery] XpHistoryQuery query, CancellationToken ct)
    {
        var result = await _service.GetXpHistoryAsync(_currentUser.UserId.Value, query, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
