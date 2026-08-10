using HanYu.Application.Features.Gamification.Admin.Achievements;
using HanYu.Application.Interfaces.Gamification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Gamification;

[ApiController]
[Route("api/v1/admin/achievements")]
[Authorize(Roles = "Admin")]
public sealed class AchievementsController : ControllerBase
{
    private readonly IGamificationAdminService _service;

    public AchievementsController(IGamificationAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _service.GetAchievementsAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAchievementRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAchievementAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateAchievementRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAchievementAsync(id, request, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        var result = await _service.DeleteAchievementAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id, CancellationToken ct)
    {
        var result = await _service.ActivateAchievementAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken ct)
    {
        var result = await _service.DeactivateAchievementAsync(id, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
