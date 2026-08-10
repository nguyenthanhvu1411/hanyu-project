using HanYu.Application.Interfaces.Gamification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Gamification;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public sealed class StreaksController : ControllerBase
{
    private readonly IGamificationAdminService _service;

    public StreaksController(IGamificationAdminService service)
    {
        _service = service;
    }

    [HttpGet("users/{userId:guid}/gamification")]
    public async Task<IActionResult> GetUserGamification(Guid userId, CancellationToken ct)
    {
        var result = await _service.GetUserProfileAsync(userId, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    // Additional admin streak endpoints for monitoring/recomputing could be added here
}
