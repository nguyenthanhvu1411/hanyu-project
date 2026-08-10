using HanYu.Application.Features.Gamification.Admin.Xp;
using HanYu.Application.Interfaces.Gamification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HanYu.API.Controllers.Admin.Gamification;

[ApiController]
[Authorize(Roles = "Admin")]
public sealed class XpController : ControllerBase
{
    private readonly IGamificationAdminService _service;

    public XpController(IGamificationAdminService service)
    {
        _service = service;
    }

    [HttpGet("api/v1/admin/xp-transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] AdminXpQuery query, CancellationToken ct)
    {
        var result = await _service.GetXpTransactionsAsync(query, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("api/v1/admin/users/{userId:guid}/xp-adjustments")]
    public async Task<IActionResult> AdjustXp(Guid userId, [FromBody] AdjustXpRequest request, CancellationToken ct)
    {
        var result = await _service.AdjustXpAsync(userId, request, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
